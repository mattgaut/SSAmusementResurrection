﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/// <summary>
/// Base class for Characters in the game.
/// Inherits ICombatant
/// </summary>
public class Character : MonoBehaviour, ICombatant {
    [SerializeField] CharacterDefinition _char_definition;

    [SerializeField] float invincibility_length = 0f;

    [SerializeField] protected bool knockback_resistant;

    [SerializeField] protected Animator anim;

    [SerializeField] protected bool is_aerial_unit;

    [SerializeField] int base_jump_count = 1;

    public Animator animator {
        get { return anim; }
    }

    public Character character { get { return this; } }
    public CharacterDefinition char_definition { get { return _char_definition; } }
    public CrowdControl crowd_control_effects { get; private set; }

    public CapStat health { get { return _char_definition.health; } }
    public Stat power { get { return _char_definition.power; } }
    public Stat armor { get { return _char_definition.armor; } }
    public Stat speed { get { return _char_definition.speed; } }
    public CapStat energy { get { return _char_definition.energy; } }
    public Stat knockback_multiplier { get { return _char_definition.knockback_modifier; } }

    public int jump_count { get { return base_jump_count + bonus_jump_count; } }
    public int bonus_jump_count { get; private set; }

    public delegate void OnHitCallback(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, IDamageable hit);
    public delegate void OnKillCallback(Character killer, ICombatant killed);
    public delegate void OnTakeDamage(Character hit_character, float pre_mitigation_damage, float post_mitigation_damage, ICombatant hit_by);
    public delegate void OnDeathCallback(Character killed, ICombatant killer);
    public delegate void OnTakeKnockback(ICombatant source, Vector2 force, float length);

    public event OnHitCallback on_hit;
    public event OnKillCallback on_kill;
    public event OnTakeDamage on_take_damage;
    public event OnDeathCallback on_death;
    public event OnTakeKnockback on_take_knockback;

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

    public bool alive {
        get; protected set;
    }
    public bool invincible {
        get { return invincibility_lock.locked; }
    }

    public bool can_input {
        get { return alive && !is_knocked_back && !crowd_control_effects.IsCCed(CrowdControl.Type.stunned); }
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

    protected ICombatant last_hit_by;

    float knockback_dissipation_time;
    Coroutine knockback_routine;
    Vector3 original_knockback_force;

    Coroutine dash_routine;

    Coroutine iframes;

    float last_cc_update;

    public virtual bool TrySpendEnergy(int cost) {
        if (energy.current >= cost) {
            energy.current -= cost;
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

    /// <summary>
    /// Deals Damage to target and calls all OnHitCallbacks with proper info
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="target"></param>
    /// <param name="trigger_on_hit">"Should this damage instance trigger on hit effects?"</param>
    /// <returns>Damage target took</returns>
    public float DealDamage(float damage, IDamageable target, bool trigger_on_hit = true) {
        float damage_dealt = target.TakeDamage(damage, this);
        if (damage_dealt > 0 && trigger_on_hit) {
            InvokeOnHit(this, damage, damage_dealt, target);

        }
        return damage_dealt;
    }

    /// <summary>
    /// If damage source is a Character or ICombatant use ICombatant.DealDamage instead 
    /// 
    /// Takes damage and calls OnTakeDamage effects.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="source"></param>
    /// <returns>Damage Taken</returns>
    public float TakeDamage(float damage, ICombatant source) {
        float old = health.current;
        float post_mitigation_damage = Mathf.Max(damage - armor, 0);
        if (post_mitigation_damage > 0) {
            last_hit_by = source;
            if (invincibility_length > 0) {
                if (iframes != null) StopCoroutine(iframes);
                iframes = StartCoroutine(IFrames());
            }
            health.current -= post_mitigation_damage;
            if (health.current <= 0) {
                Die(source);
            } else {
                InvokeOnTakeDamage(this, damage, post_mitigation_damage, source);
            }
        }
        return old - health.current;
    }

    /// <summary>
    ///  Invokes KnockBack routine and cancels dashes.
    /// </summary>
    /// <param name="source">ICombatant that initiated knockback if any</param>
    /// <param name="force">The force of the knockback</param>
    /// <param name="length">Knockback Duration</param>
    public void TakeKnockback(ICombatant source, Vector2 force, float length = 0.5f) {
        if (knockback_resistant) {
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
    public void GiveKnockback(IDamageable target, Vector2 force, float length = 0.5f) {
        target.TakeKnockback(this, force * knockback_multiplier, length);
    }

    /// <summary>
    /// Initiates Dash Routine
    /// </summary>
    /// <param name="dash">Dash distance and direction</param>
    /// <param name="time">Dash Duration</param>
    public void Dash(Vector2 dash, float time) {
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
    public virtual void GiveKillCredit(ICombatant killed) {
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
        body.AddForce(Quaternion.Euler(0, 0, angle) * Vector2.up * 8f, ForceMode2D.Impulse);
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
        knockback_force = new Vector2(knockback_force.x, 0);
    }

    /// <summary>
    /// Cancels knockback in horizontal axis
    /// </summary>
    public void CancelXKnockBack() {
        knockback_force = new Vector2(0, knockback_force.y);
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

    protected void InvokeOnHit(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, IDamageable hit) {
        on_hit?.Invoke(hitter, pre_mitigation_damage, post_mitigation_damage, hit);
    }
    protected void InvokeOnTakeDamage(Character hit_character, float pre_mitigation_damage, float post_mitigation_damage, ICombatant hit_by) {
        on_take_damage?.Invoke(hit_character, pre_mitigation_damage, post_mitigation_damage, hit_by);
    }
    protected void InvokeOnKill(Character killer, ICombatant killed) {
        on_kill?.Invoke(killer, killed);
    }
    protected void InvokeOnDeath(Character killed, ICombatant killed_by) {
        on_death?.Invoke(killed, killed_by);
    }

    protected void Awake() {
        health.current = health.max;
        energy.current = energy.max;

        crowd_control_effects = new CrowdControl();

        alive = true;

        movement_lock = new Lock();
        anti_gravity_lock = new Lock();
        invincibility_lock = new Lock();

        OnAwake();
    }

    /// <summary>
    /// Overload this method instead of hiding Awake
    /// </summary>
    protected virtual void OnAwake() { }

    protected void Start() {
        OnStart();
    }

    /// <summary>
    /// Overload this method instead of hiding Start
    /// </summary>
    protected virtual void OnStart() {

    }

    void Update() {
        crowd_control_effects.Update(Time.time - last_cc_update);
        last_cc_update = Time.time;
    }

    void FixedUpdate() {
        crowd_control_effects.Update(Time.time - last_cc_update);
        last_cc_update = Time.time;
    }


    protected virtual void Die(ICombatant killed_by) {
        last_hit_by.GiveKillCredit(this);
        InvokeOnDeath(this, killed_by);
        Destroy(gameObject);
    }

    /// <summary>
    /// Makes Player temporarily invincible
    /// </summary>
    /// <returns>IEnumerator</returns>
    protected virtual IEnumerator IFrames() {
        float time = 0;
        int lock_value = invincibility_lock.AddLock();
        while (time < invincibility_length) {
            time += Time.fixedDeltaTime;
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
        is_knocked_back = true;
        knockback_dissipation_time = length;

        while (knockback_dissipation_time > 0 && is_knocked_back) {
            float time_step = Mathf.Min(Time.deltaTime, knockback_dissipation_time);

            Vector2 old_force = force * Mathf.Pow(knockback_dissipation_time / length, 3f);
            knockback_dissipation_time -= time_step;
            Vector2 current_force = force * Mathf.Pow(knockback_dissipation_time / length, 3f);

            knockback_force += old_force - current_force;
            yield return null;
        }
        knockback_force = Vector2.zero;

        if (!is_aerial_unit) {
            //while (is_knocked_back) {
            //    yield return null;
            //}
            is_knocked_back = false;
        } else {
            is_knocked_back = false;
        }
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
        while (is_dashing && timer > 0) {
            float time_step = Mathf.Min(Time.deltaTime, timer);
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
        while (is_dashing && timer > 0) {
            float time_step = Mathf.Min(Time.fixedDeltaTime, timer);
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
public struct CharacterDefinition {

    [SerializeField] string _name;

    [SerializeField] CapStat _health;
    [SerializeField] Stat _power;
    [SerializeField] Stat _armor;
    [SerializeField] Stat _speed;
    [SerializeField] CapStat _energy;
    [SerializeField] Stat _knockback_modifier;

    [SerializeField] Transform _center_mass, _feet, _head;

    public Transform center_mass { get { return _center_mass; } }
    public Transform feet { get { return _feet; } }
    public Transform head { get { return _head; } }

    public string name { get { return _name; } }

    public CapStat health { get { return _health; } }
    public Stat power { get { return _power; } }
    public Stat armor { get { return _armor; } }
    public Stat speed { get { return _speed; } }
    public CapStat energy { get { return _energy; } }
    public Stat knockback_modifier { get { return _knockback_modifier; } }

}