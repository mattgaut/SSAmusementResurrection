using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ICombatant {
    [SerializeField] CharacterDefinition _char_definition;

    [SerializeField] float invincibility_length = 0f;

    [SerializeField] protected bool knockback_resistant;

    [SerializeField] protected Animator anim;

    public Animator animator {
        get { return anim; }
    }

    public virtual Ability basic_ability { get { return null; } }
    public virtual Ability ability_1 { get { return null; } }
    public virtual Ability ability_2 { get { return null; } }
    public virtual Ability ability_3 { get { return null; } }

    public virtual bool can_use_basic_ability {
        get { return has_basic_ability; }
    }
    public virtual bool can_use_skill_1 {
        get { return has_ability_1; }
    }
    public virtual bool can_use_skill_2 {
        get { return has_ability_2; }
    }
    public virtual bool can_use_skill_3 {
        get { return has_ability_3; }
    }

    public virtual bool has_basic_ability { get { return basic_ability != null; } }
    public virtual bool has_ability_1 { get { return ability_1 != null; } }
    public virtual bool has_ability_2 { get { return ability_2 != null; } }
    public virtual bool has_ability_3 { get { return ability_3 != null; } }

    public CharacterDefinition char_definition { get { return _char_definition; } }

    public CapStat health { get { return _char_definition.health; } }
    public Stat power { get { return _char_definition.power; } }
    public Stat armor { get { return _char_definition.armor; } }
    public Stat speed { get { return _char_definition.speed; } }
    public CapStat energy { get { return _char_definition.energy; } }

    public delegate void OnHitCallback(Character player, float pre_mitigation_damage, float post_mitigation_damage, IDamageable hit);
    public delegate void OnKillCallback(Character player, ICombatant killed);
    public delegate void OnTakeDamage(Character player, float pre_mitigation_damage, float post_mitigation_damage, ICombatant hit_by);

    public Vector3 knockback_force {
        get; private set;
    }
    public bool knocked_back {
        get; private set;
    }

    public bool alive {
        get; protected set;
    }
    public bool invincible {
        get; private set;
    }

    public bool can_input {
        get { return alive && !knocked_back; }
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

    Coroutine iframes;

    public virtual bool TrySpendEnergy(int cost) {
        if (energy.current >= cost) {
            energy.current -= cost;
            return true;
        }
        return false;
    }

    public float DealDamage(float damage, IDamageable target, bool trigger_on_hit = true) {
        float damage_dealt = target.TakeDamage(damage, this);
        if (damage_dealt > 0 && trigger_on_hit) {
            foreach (OnHitCallback oh in on_hits) {
                oh(this, damage, damage_dealt, target);
            }
        }
        return damage_dealt;
    }

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

    public void TakeKnockback(ICombatant source, Vector3 force, float length = 0.5f) {
        if (knockback_resistant) {
            return;
        }

        knockback_force = force;
        knockback_dissipation_time = length;
        if (knockback_routine != null) {
            StopCoroutine(knockback_routine);
        }
        knockback_routine = StartCoroutine(KnockbackRoutine(force, length));
    }

    public virtual void GiveKillCredit(ICombatant killed) {
        foreach (OnKillCallback ok in on_kills) {
            ok(this, killed);
        }
    }

    public void DropObject(GameObject obj) {
        obj.transform.position = transform.position + Vector3.up * 0.5f;
        float angle = Random.Range(0f, 90f) - 45f;
        Rigidbody2D body = obj.GetComponent<Rigidbody2D>();
        body.AddForce(Quaternion.Euler(0, 0, angle) * Vector2.up * 8f, ForceMode2D.Impulse);
    }

    public virtual void LogBuff(Buff b) { }

    public void CancelKnockBack() {
        knocked_back = false;
    }
    public void CancelYKnockBack() {
        knockback_force = new Vector3(knockback_force.x, 0, knockback_force.z);
    }
    public void CancelXKnockBack() {
        knockback_force = new Vector3(0, knockback_force.y, knockback_force.z);
    }

    public bool CheckCancelVelocity() {
        bool to_return = cancel_velocity;
        cancel_velocity = false;
        return to_return;
    }

    public void CancelVelocity() {
        cancel_velocity = true;
    }

    public void LockMovement() { movement_locks++; }
    public void UnlockMovement() { movement_locks--; }

    public void LockGrav() { anti_grav_locks++; }
    public void UnlockGrav() { anti_grav_locks--; }

    public void AddOnKill(OnKillCallback ok) { on_kills.Add(ok); }
    public void RemoveOnKill(OnKillCallback ok) { on_kills.Remove(ok); }

    public void AddOnHit(OnHitCallback oh) { on_hits.Add(oh); }
    public void RemoveOnHit(OnHitCallback oh) { on_hits.Remove(oh); }

    public void AddTakeDamage(OnTakeDamage otd) { on_take_damages.Add(otd); }
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

    protected virtual void OnAwake() { }

    protected void Start() {
        OnStart();
    }

    protected virtual void OnStart() {

    }


    protected virtual void Die() {
        last_hit_by.GiveKillCredit(this);

        Destroy(gameObject);
    }

    protected virtual IEnumerator IFrames() {
        float time = 0;
        invincible = true;
        while (time < invincibility_length) {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        invincible = false;
    }

    IEnumerator KnockbackRoutine(Vector3 force, float length) {
        knocked_back = true;

        while (knockback_dissipation_time > 0) {
            knockback_force = force * Mathf.Pow(knockback_dissipation_time / length, 0.8f);
            knockback_dissipation_time -= Time.fixedDeltaTime;
            if (knocked_back == false) {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        knockback_force = Vector3.zero;
        while (knocked_back) {
            yield return new WaitForFixedUpdate();
        }
    }
}

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
