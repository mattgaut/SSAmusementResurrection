using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActiveAbility : Ability {

    public override Type ability_type {
        get { return Type.Active; }
    }
    public override ActiveAbility active {
        get { return this; }
    }

    public float cooldown { get { return _cooldown; } }

    public bool on_cooldown { get { return time_cooldown_ends > Time.time; } }
    public bool using_ability { get; protected set; }
    public override bool available {
        get {
            return (can_use == null || can_use.Invoke()) && !on_cooldown; 
        }
    }

    public float time_cooldown_ends { get; private set; }
    public float time_until_cooldown_ends { get { return time_cooldown_ends - Time.time; } }

    public int energy_cost { get { return _energy_cost; } }

    public delegate bool CanUse();
    CanUse can_use;

    [SerializeField] int _energy_cost;
    [SerializeField] float _cooldown;

    [SerializeField] Events events;

    public void SetCooldown(float cooldown) {
        _cooldown = cooldown;
    }

    public void SetCanUse(CanUse can_use) {
        this.can_use = can_use;
    }

    public bool TryUse(float input = 0) {
        if (available && character.TrySpendEnergy(_energy_cost)) {
            PutOnCooldown();
            UseAbility(input);
            return true;
        } else {
            return false;
        }
    }

    protected void PutOnCooldown() {
        time_cooldown_ends = Time.time + _cooldown;
        events.on_begin_cooldown.Invoke();
    }

    protected abstract void UseAbility(float input);

    [System.Serializable]
    class Events {
        [SerializeField] UnityEvent _on_begin_cooldown;

        public UnityEvent on_begin_cooldown { get { return _on_begin_cooldown; } }
    }
}
