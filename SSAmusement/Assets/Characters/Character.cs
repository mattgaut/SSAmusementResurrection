﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/// <summary>
/// Base class for Characters in the game.
/// Inherits Character
/// </summary>
public class Character : MonoBehaviour {
    public enum Team { player, enemy }

    [SerializeField] CharacterID _id;

    [SerializeField] CharacterStats _stats;

    [SerializeField] float invincibility_length = 0f;

    [SerializeField] protected bool knockback_resistant;

    [SerializeField] protected AnimatorWrapper anim;

    [SerializeField] protected bool is_aerial_unit;

    [SerializeField] protected AbilitySet _abilities;

    [SerializeField] int base_jump_count = 1;

    public CharacterID id {
        get { return _id; }
    }

    public Team team {
        get; private set;
    }

    public AnimatorWrapper animator {
        get { return anim; }
    }
    
    public CharacterStats stats { get { return _stats; } }
    public CrowdControl crowd_control_effects { get; private set; }

    public CapStat health { get { return _stats.health; } }
    public Stat power { get { return _stats.power; } }
    public Stat armor { get { return _stats.armor; } }
    public Stat speed { get { return _stats.speed; } }
    public CapStat energy { get { return _stats.energy; } }
    public Stat knockback_multiplier { get { return _stats.knockback_modifier; } }

    public AbilitySet abilities { get { return _abilities; } }

    public int jump_count { get { return base_jump_count + bonus_jump_count; } }
    public int bonus_jump_count { get; private set; }

    public delegate void OnHitCallback(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit);
    public delegate void OnKillCallback(Character killer, Character killed);
    public delegate void OnTakeDamage(Character hit_character, float pre_mitigation_damage, float post_mitigation_damage, Character hit_by);
    public delegate void OnDeathCallback(Character killed, Character killer);
    public delegate void OnTakeKnockback(Character source, Vector2 force, float length);
    public delegate void OnSpendEnergy(Character source, float spent);
    public delegate void OnWasHitByAttack(Character source, bool hit_successful);

    public event OnHitCallback on_deal_damage;
    public event OnHitCallback on_landed_hit;
    public event OnKillCallback on_kill;
    public event OnTakeDamage on_take_damage;
    public event OnDeathCallback on_death;
    public event OnTakeKnockback on_take_knockback;
    public event OnSpendEnergy on_spend_energy;
    public event OnWasHitByAttack on_was_hit;

    public Vector2 knockback_force {
        get; set;
    }
    public bool is_knocked_back {
        get; private set;
    }

    public Vector2 dash_force {
        get; set;
    }
    public bool is_dashing {
        get; private set;
    }

    public bool is_alive {
        get; protected set;
    }
    public bool invincible {
        get { return invincibility_lock.locked; }
    }

    public bool can_input {
        get { return is_alive && !is_knocked_back && !crowd_control_effects.IsCCed(CrowdControl.Type.stunned); }
    }
    public bool can_use_skills {
        get { return is_alive && !is_knocked_back && !crowd_control_effects.IsCCed(CrowdControl.Type.stunned); }
    }
    public bool can_move {
        get { return movement_lock.unlocked && !crowd_control_effects.IsCCed(CrowdControl.Type.snared, CrowdControl.Type.stunned); }
    }
    public bool cancel_velocity {
        get; private set;
    }
    public bool anti_gravity {
        get { return anti_gravity_lock.locked; }
    }

    protected Lock movement_lock;
    protected Lock anti_gravity_lock;
    protected Lock invincibility_lock;

    protected Character last_hit_by;

    protected bool can_continue_dash { get { return !crowd_control_effects.IsCCed(CrowdControl.Type.stunned); } }

    float knockback_dissipation_time;
    Coroutine knockback_routine;
    Vector2 current_total_knockback;

    Coroutine dash_routine;

    Coroutine iframes;

    float last_cc_update;

    public virtual bool TrySpendEnergy(int cost) {
        if (cost <= 0) {
            return true;
        }
        if (energy.current >= cost) {
            energy.current -= cost;
            on_spend_energy?.Invoke(this, cost);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Restore Health to the character
    /// </summary>
    /// <param name="restore">Amount of health to restore</param>
    /// <returns>Total health restored</returns>
    public float RestoreHealth(float restore) {
        float old = health.current;
        health.current += restore;
        return health.current - old;
    }

    /// <summary>
    /// Restore Energy to the character
    /// </summary>
    /// <param name="restore">Amount of Energy to restore</param>
    /// <returns>Total Energy restored</returns>
    public float RestoreEnergy(float restore) {
        float old = energy.current;
        energy.current += restore;
        return energy.current - old;
    }

    public void Execute(Character executioner) {
        executioner.InvokeOnDealDamage(this, health, health, executioner);
        Die(executioner);        
    }

    /// <summary>
    /// Deals Damage to target and calls all OnHitCallbacks with proper info
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="target"></param>
    /// <param name="trigger_on_hit">"Should this damage instance trigger on hit effects?"</param>
    /// <returns>Damage target took</returns>
    public float DealDamage(float damage, Character target, bool trigger_on_hit = true) {
        float damage_dealt = target.TakeDamage(damage, this);
        if (damage_dealt > 0) {
            if (trigger_on_hit) InvokeOnHit(this, damage, damage_dealt, target);
            InvokeOnDealDamage(this, damage, damage_dealt, target);
        }     
        return damage_dealt;
    }

    /// <summary>
    /// If damage source is a Character use Character.DealDamage instead 
    /// 
    /// Takes damage and calls OnTakeDamage effects.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="source"></param>
    /// <returns>Damage Taken</returns>
    public float TakeDamage(float damage, Character source, bool is_true_damage = false) {
        float old = health.current;
        float post_mitigation_damage = is_true_damage ? damage : MitigateDamage(damage);
        if (post_mitigation_damage > 0) {
            last_hit_by = source;
            if (invincibility_length > 0) {
                if (iframes != null) StopCoroutine(iframes);
                iframes = StartCoroutine(IFrames());
            }
            health.current -= post_mitigation_damage;

            InvokeOnTakeDamage(this, damage, post_mitigation_damage, source);
            if (health.current <= 0) {
                Die(source);
            }
        }
        return old - health.current;
    }

    /// <summary>
    /// Used to check if an attack is successful on the Character. 
    /// </summary>
    /// <param name="source">Character that attacked</param>
    /// <returns>Whether attack was successful and should be registered.</returns>
    public bool WasHit(Character source) {
        bool was_hit = IsHitSuccessful(source);

        InvokeOnWasHit(source, was_hit);

        return was_hit;
    }

    /// <summary>
    ///  Invokes KnockBack routine and cancels dashes.
    /// </summary>
    /// <param name="source">Character that initiated knockback if any</param>
    /// <param name="force">The force of the knockback</param>
    /// <param name="length">Knockback Duration</param>
    public void TakeKnockback(Character source, Vector2 force, float length = 0.5f) {
        if (knockback_resistant || force == Vector2.zero) {
            return;
        }
        if (is_dashing) {
            EndDash();
        }
        if (is_aerial_unit) {
            force *= 2;
            length *= 2;
        }

        if (knockback_routine != null) {
            StopCoroutine(knockback_routine);
        }

        on_take_knockback?.Invoke(source, force, length);
        knockback_routine = StartCoroutine(KnockbackRoutine(force, length));
    }

    /// <summary>
    /// Calls Takeknockback on target with specified parameters modified
    /// by characters stats
    /// </summary>
    /// <param name="target">IDamageable that is getting knockedback</param>
    /// <param name="force">The force of the knockback</param>
    /// <param name="length">Knockback Duration</param>
    public void GiveKnockback(Character target, Vector2 force, float length = 0.5f) {
        target.TakeKnockback(this, force * knockback_multiplier, length);
    }

    /// <summary>
    /// Initiates Dash Routine
    /// </summary>
    /// <param name="dash">Dash distance and direction</param>
    /// <param name="time">Dash Duration</param>
    public void Dash(Vector2 dash, float time) {
        if (is_knocked_back) return;
        if (dash_routine != null) {
            EndDash();
        }
        dash_routine = StartCoroutine(DashRoutine(dash, time));
    }

    public void Dash(CustomDash dash) {
        if (dash_routine != null) {
            EndDash();
        }
        dash_routine = StartCoroutine(CustomDashRoutine(dash));
    }


    /// <summary>
    /// Invokes OnKillCallbacks
    /// </summary>
    /// <param name="killed"></param>
    public virtual void GiveKillCredit(Character killed) {
        InvokeOnKill(this, killed);
    }


    /// <summary>
    /// Lightly shoots an object out of character in upward arc
    /// Gameobject must have rigidbody
    /// </summary>
    /// <param name="obj">Object to Shoot</param>
    /// <param name="should_instantiate_copy">Should Character handle object instantiation?</param>
    public void DropObject(GameObject obj, bool should_instantiate_copy = false) {
        if (should_instantiate_copy) {
            obj = Instantiate(obj);
        }
        obj.transform.position = transform.position + Vector3.up * 0.5f;
        float angle = Random.Range(0f, 90f) - 45f;
        Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
        body.AddForce(Quaternion.Euler(0, 0, angle) * Vector2.up * Random.Range(8f, 20f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Adds buff to buff tracker if one exists
    /// </summary>
    /// <param name="buff"></param>
    public virtual void LogBuff(IBuff buff) { }

    public void CancelKnockBack() {
        is_knocked_back = false;
    }

    /// <summary>
    /// Cancels knockback in vertical axis
    /// </summary>
    public void CancelYKnockBack() {
        current_total_knockback = new Vector2(current_total_knockback.x, 0);
    }

    /// <summary>
    /// Cancels knockback in horizontal axis
    /// </summary>
    public void CancelXKnockBack() {
        current_total_knockback = new Vector2(0, current_total_knockback.y);
    }



    /// <summary>
    /// Checks if character wants to cancel velocity
    /// Set should clear to false if not handling velocity cancellation
    /// </summary>
    /// <param name="should_clear">Should clear cancel velocity flag? (Should be false if not handling cancellation)</param>
    /// <returns>Cancel Velocity flag</returns>
    public bool CheckCancelVelocityFlag(bool should_clear = true) {
        bool to_return = cancel_velocity;
        if (should_clear) cancel_velocity = false;
        return to_return;
    }

    /// <summary>
    /// Set Cancel Velocity Flag to true
    /// </summary>
    public void RaiseCancelVelocityFlag() {
        cancel_velocity = true;
    }

    /// <summary>
    /// Adds lock to character movement
    /// </summary>
    public int LockMovement() { return movement_lock.AddLock(); }
    /// <summary>
    /// Removes lock from character movement
    /// </summary>
    public bool UnlockMovement(int lock_value) { return movement_lock.RemoveLock(lock_value); }

    /// <summary>
    /// Adds lock to character Invincibility
    /// </summary>
    public int LockInvincibility() { return invincibility_lock.AddLock(); }
    /// <summary>
    /// Removes lock from character movement
    /// </summary>
    public bool UnlockInvincibility(int lock_value) { return invincibility_lock.RemoveLock(lock_value); }

    /// <summary>
    /// Adds lock to character gravity
    /// </summary>
    public int LockGravity() { return anti_gravity_lock.AddLock(); }

    /// <summary>
    /// Removes lock from character gravity
    /// </summary>
    public bool UnlockGravity(int lock_value) { return anti_gravity_lock.RemoveLock(lock_value); }

    public void AddBonusJump(int add) {
        bonus_jump_count += add;
    }

    public void RemoveBonusJump(int subtract) {
        bonus_jump_count -= subtract;
        bonus_jump_count = Mathf.Max(bonus_jump_count, 0);
    }

    protected void InvokeOnHit(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        on_landed_hit?.Invoke(hitter, pre_mitigation_damage, post_mitigation_damage, hit);
    }
    protected void InvokeOnDealDamage(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        on_deal_damage?.Invoke(hitter, pre_mitigation_damage, post_mitigation_damage, hit);
    }
    protected void InvokeOnTakeDamage(Character hit_character, float pre_mitigation_damage, float post_mitigation_damage, Character hit_by) {
        on_take_damage?.Invoke(hit_character, pre_mitigation_damage, post_mitigation_damage, hit_by);
    }
    protected void InvokeOnKill(Character killer, Character killed) {
        on_kill?.Invoke(killer, killed);
    }
    protected void InvokeOnDeath(Character killed, Character killed_by) {
        on_death?.Invoke(killed, killed_by);
    }
    protected void InvokeOnWasHit(Character source, bool was_hit_successful) {
        on_was_hit?.Invoke(source, was_hit_successful);
    }

    protected void Awake() {
        health.current = health.max;
        energy.current = energy.max;

        crowd_control_effects = new CrowdControl();

        is_alive = true;

        movement_lock = new Lock();
        anti_gravity_lock = new Lock();
        invincibility_lock = new Lock();

        team = id.type;

        OnAwake();
    }


    /// <summary>
    /// Overload this method instead of hiding Awake
    /// </summary>
    protected virtual void OnAwake() { }

    protected void Start() {
        if (abilities == null) {
            _abilities = GetComponentInChildren<AbilitySet>();
        }
        abilities?.SetCharacter(this);

        GameManager.instance.AddOnTimeScaleChangedEvent(team, OnTimeScaleChanged);
        OnTimeScaleChanged(GameManager.instance.GetTeamTimeScale(team));

        OnStart();
    }

    /// <summary>
    /// Overload this method instead of hiding Start
    /// </summary>
    protected virtual void OnStart() {

    }

    protected virtual void OnTimeScaleChanged(float new_time_scale) {
        anim.speed = new_time_scale;
    }

    void Update() {
        crowd_control_effects.Update(Time.time - last_cc_update);
        last_cc_update = Time.time;
    }

    void FixedUpdate() {
        crowd_control_effects.Update(Time.time - last_cc_update);
        last_cc_update = Time.time;
    }

    protected virtual bool IsHitSuccessful(Character source) {
        return !invincible;
    }

    protected virtual void Die(Character killed_by) {
        is_alive = false;
        GameManager.instance.RemoveOnTimeScaleChangedEvent(team, OnTimeScaleChanged);
        last_hit_by.GiveKillCredit(this);
        InvokeOnDeath(this, killed_by);
        Destroy(gameObject);
    }

    protected virtual float MitigateDamage(float damage) {
        damage *= 9 / (9 + armor);

        return damage;
    }

    /// <summary>
    /// Makes Player temporarily invincible
    /// </summary>
    /// <returns>IEnumerator</returns>
    protected virtual IEnumerator IFrames() {
        float time = 0;
        int lock_value = invincibility_lock.AddLock();
        while (time < invincibility_length) {
            time += GameManager.GetFixedDeltaTime(team);
            yield return new WaitForFixedUpdate();
        }
        invincibility_lock.RemoveLock(lock_value);
    }

    /// <summary>
    /// Applies a dissipating knockback force to the character
    /// </summary>
    /// <param name="force">Initial force</param>
    /// <param name="length">Maximum Length of force application</param>
    /// <returns>Ienumerator</returns>
    IEnumerator KnockbackRoutine(Vector2 force, float length) {
        current_total_knockback = force;
        is_knocked_back = true;
        knockback_dissipation_time = length;

        while (knockback_dissipation_time > 0 && is_knocked_back) {
            float time_step = Mathf.Min(GameManager.GetDeltaTime(team), knockback_dissipation_time);

            Vector2 old_force = current_total_knockback * Mathf.Pow(knockback_dissipation_time / length, 3f);
            knockback_dissipation_time -= time_step;
            Vector2 current_force = current_total_knockback * Mathf.Pow(knockback_dissipation_time / length, 3f);

            knockback_force += old_force - current_force;
            yield return null;
        }
        knockback_force = Vector2.zero;

        is_knocked_back = false;
    }

    /// <summary>
    /// Applies a fixed dash force to the character
    /// </summary>
    /// <param name="dash">Dash force</param>
    /// <param name="length">Dash duration</param>
    /// <returns>IEnumerator</returns>
    IEnumerator DashRoutine(Vector2 dash, float length) {
        is_dashing = true;

        float timer = length;
        while (is_dashing && timer > 0 && !crowd_control_effects.IsCCed(CrowdControl.Type.stunned)) {
            float time_step = Mathf.Min(GameManager.GetDeltaTime(team), timer);

            timer -= time_step;
            dash_force += dash * (time_step / length);
            yield return null;
        }

        EndDash();
    }

    IEnumerator CustomDashRoutine(CustomDash dash) {
        is_dashing = true;

        float timer = dash.length;
        Vector2 dash_start = transform.position;
        while (is_dashing && timer > 0 && !crowd_control_effects.IsCCed(CrowdControl.Type.stunned)) {
            float time_step = Mathf.Min(GameManager.GetFixedDeltaTime(team), timer);

            timer -= time_step;
            dash_force += dash.GetNext(dash.length - timer);
            yield return new WaitForFixedUpdate();
        }

        EndDash();
    }

    void EndDash() {
        if (dash_routine != null) StopCoroutine(dash_routine);
        dash_routine = null;
        dash_force = Vector2.zero;
        is_dashing = false;
    }

    private void OnDisable() {
        is_dashing = false;
        is_knocked_back = false;
        invincibility_lock.Clear();
    }

    private void OnDestroy() {
        if (GameManager.has_instance) GameManager.instance.RemoveOnTimeScaleChangedEvent(team, OnTimeScaleChanged);
    }

    public class CustomDash {
        public System.Func<float, float, Vector2> dash_callback { get; private set; }
        public float length { get; private set; }

        float last_time;
        Vector2 scale;

        public CustomDash(System.Func<float, float, Vector2> callback, float length, Vector2 scale) {
            dash_callback = callback;
            this.length = length;
            last_time = 0;
            this.scale = scale;
        }

        public CustomDash(System.Func<float, float, Vector2> callback, float length) {
            dash_callback = callback;
            this.length = length;
            last_time = 0;
            scale = Vector2.one;
        }

        public Vector2 GetNext(float time) {
            if (time > length) {
                time = length;
            }
            Vector2 to_return = dash_callback(last_time, time);
            last_time = time;
            return to_return * scale;
        }
    }
}

/// <summary>
/// Outlines the base stats of a character as well as providing
/// transforms to target different places of a character.
/// </summary>
[System.Serializable]
public class CharacterStats {

    [SerializeField] CapStat _health;
    [SerializeField] Stat _power;
    [SerializeField] Stat _armor;
    [SerializeField] Stat _speed;
    [SerializeField] CapStat _energy;
    [SerializeField] Stat _knockback_modifier = new Stat(1);
    [SerializeField] Stat _cooldown_reduction = new Stat(0);
    [SerializeField] Stat _attack_speed = new Stat(1);

    [SerializeField] Transform _center_mass, _feet, _head;

    public Transform center_mass { get { return _center_mass; } }
    public Transform feet { get { return _feet; } }
    public Transform head { get { return _head; } }

    public CapStat health { get { return _health; } }
    public Stat power { get { return _power; } }
    public Stat armor { get { return _armor; } }
    public Stat speed { get { return _speed; } }
    public CapStat energy { get { return _energy; } }
    public Stat knockback_modifier { get { return _knockback_modifier; } }
    public Stat attack_speed { get { return _attack_speed; } }
    public Stat cooldown_reduction { get { return _cooldown_reduction; } }

}