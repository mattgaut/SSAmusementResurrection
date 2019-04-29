using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for Characters in the game.
/// Inherits ICombatant
/// </summary>
public class Character : MonoBehaviour, ICombatant {
    [SerializeField] CharacterDefinition _char_definition;

    [SerializeField] AbilitySet _abilities;

    [SerializeField] float invincibility_length = 0f;

    [SerializeField] protected bool knockback_resistant;

    [SerializeField] protected Animator anim;

    [SerializeField] protected bool is_aerial_unit;

    public Animator animator {
        get { return anim; }
    }

    public AbilitySet abilities { get { return _abilities; } }

    public Character character { get { return this; } }
    public CharacterDefinition char_definition { get { return _char_definition; } }

    public CapStat health { get { return _char_definition.health; } }
    public Stat power { get { return _char_definition.power; } }
    public Stat armor { get { return _char_definition.armor; } }
    public Stat speed { get { return _char_definition.speed; } }
    public CapStat energy { get { return _char_definition.energy; } }

    public delegate void OnHitCallback(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, IDamageable hit);
    public delegate void OnKillCallback(Character killer, ICombatant killed);
    public delegate void OnTakeDamage(Character hit_character, float pre_mitigation_damage, float post_mitigation_damage, ICombatant hit_by);

    public Vector3 knockback_force {
        get; private set;
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
        get; private set;
    }

    public bool can_input {
        get { return alive && !is_knocked_back; }
    }
    public bool can_move {
        get { return movement_locks == 0; }
    }
    public int movement_locks {
        get; private set;
    }
    public bool cancel_velocity {
        get; private set;
    }
    public int anti_grav_locks {
        get; private set;
    }
    public bool anti_grav {
        get { return anti_grav_locks > 0; }
    }

    protected List<OnHitCallback> on_hits;
    protected List<OnKillCallback> on_kills;
    protected List<OnTakeDamage> on_take_damages;

    protected ICombatant last_hit_by;

    float knockback_dissipation_time;
    Coroutine knockback_routine;
    Vector3 original_knockback_force;

    Coroutine dash_routine;

    Coroutine iframes;

    public virtual bool TrySpendEnergy(int cost) {
        if (energy.current >= cost) {
            energy.current -= cost;
            return true;
        }
        return false;
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
            foreach (OnHitCallback oh in on_hits) {
                oh(this, damage, damage_dealt, target);
            }
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
                Die();
            } else {
                foreach (OnTakeDamage otd in on_take_damages) {
                    otd(this, damage, post_mitigation_damage, source);
                }
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
    public void TakeKnockback(ICombatant source, Vector3 force, float length = 0.5f) {
        if (knockback_resistant) {
            return;
        }
        if (is_dashing) {
            EndDash();
        }

        if (knockback_routine != null) {
            StopCoroutine(knockback_routine);
        }
        knockback_routine = StartCoroutine(KnockbackRoutine(force, length));
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


    /// <summary>
    /// Invokes OnKillCallbacks
    /// </summary>
    /// <param name="killed"></param>
    public virtual void GiveKillCredit(ICombatant killed) {
        foreach (OnKillCallback ok in on_kills) {
            ok(this, killed);
        }
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
    public virtual void LogBuff(Buff buff) { }

    public void CancelKnockBack() {
        is_knocked_back = false;
    }

    /// <summary>
    /// Cancels knockback in vertical axis
    /// </summary>
    public void CancelYKnockBack() {
        knockback_force = new Vector3(knockback_force.x, 0, knockback_force.z);
    }

    /// <summary>
    /// Cancels knockback in horizontal axis
    /// </summary>
    public void CancelXKnockBack() {
        knockback_force = new Vector3(0, knockback_force.y, knockback_force.z);
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


    // TODO: Make Locks give and require code when locking and unlocking

    /// <summary>
    /// Adds lock to character movement
    /// </summary>
    public void LockMovement() { movement_locks++; }
    /// <summary>
    /// Removes lock from character movement
    /// </summary>
    public void UnlockMovement() { movement_locks--; }

    /// <summary>
    /// Adds lock to character gravity
    /// </summary>
    public void LockGravity() { anti_grav_locks++; }

    /// <summary>
    /// Removes lock from character gravity
    /// </summary>
    public void UnlockGravity() { anti_grav_locks--; }

    /// <summary>
    /// Adds OnKillCallback to character
    /// </summary>
    /// <param name="ok">OnKillCallback or equivalent</param>
    public void AddOnKill(OnKillCallback ok) { on_kills.Add(ok); }
    /// <summary>
    /// Removes OnKillCallback from character
    /// </summary>
    /// <param name="ok">OnKillCallback or equivalent</param>
    public void RemoveOnKill(OnKillCallback ok) { on_kills.Remove(ok); }

    /// <summary>
    /// Adds OnHitCallback to character
    /// </summary>
    /// <param name="oh">OnHitCallback or equivalent</param>
    public void AddOnHit(OnHitCallback oh) { on_hits.Add(oh); }
    /// <summary>
    /// Removes OnHitCallback from character
    /// </summary>
    /// <param name="oh">OnHitCallback or equivalent</param>
    public void RemoveOnHit(OnHitCallback oh) { on_hits.Remove(oh); }

    /// <summary>
    /// Adds OnTakeDamage to character
    /// </summary>
    /// <param name="otd">OnTakeDamage or equivalent</param>
    public void AddOnTakeDamage(OnTakeDamage otd) { on_take_damages.Add(otd); }
    /// <summary>
    /// Removes OnTakeDamage from character
    /// </summary>
    /// <param name="otd">OnTakeDamage or equivalent</param>
    public void RemoveOnTakeDamage(OnTakeDamage otd) { on_take_damages.Remove(otd); }

    protected void Awake() {
        health.current = health.max;
        energy.current = energy.max;

        on_hits = new List<OnHitCallback>();
        on_kills = new List<OnKillCallback>();
        on_take_damages = new List<OnTakeDamage>();

        alive = true;

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


    protected virtual void Die() {
        last_hit_by.GiveKillCredit(this);

        Destroy(gameObject);
    }

    /// <summary>
    /// Makes Player temporarily invincible
    /// </summary>
    /// <returns>IEnumerator</returns>
    protected virtual IEnumerator IFrames() {
        float time = 0;
        invincible = true;
        while (time < invincibility_length) {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        invincible = false;
    }

    /// <summary>
    /// Applies a dissipating knockback force to the character
    /// </summary>
    /// <param name="force">Initial force</param>
    /// <param name="length">Maximum Length of force application</param>
    /// <returns>Ienumerator</returns>
    IEnumerator KnockbackRoutine(Vector3 force, float length) {
        is_knocked_back = true;
        knockback_force = force;
        knockback_dissipation_time = length;

        while (knockback_dissipation_time > 0 && is_knocked_back) {
            knockback_force = force * Mathf.Pow(knockback_dissipation_time / length, 0.8f);
            knockback_dissipation_time -= Time.deltaTime;
            yield return null;
        }
        knockback_force = Vector3.zero;

        if (!is_aerial_unit) {
            while (is_knocked_back) {
                yield return null;
            }
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
            float time_step = Time.fixedDeltaTime;
            dash_force += dash * (time_step / length);
            timer -= time_step;
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

}