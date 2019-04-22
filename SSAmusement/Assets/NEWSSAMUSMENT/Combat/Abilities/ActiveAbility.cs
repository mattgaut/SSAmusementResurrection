using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAbility : Ability {

    public bool on_cooldown { get { return time_off_cooldown > Time.time; } }
    public bool using_ability { get; protected set; }

    public float time_off_cooldown { get; private set; }

    public int energy_cost { get { return _energy_cost; } }

    public delegate bool CanUse();
    CanUse can_use;

    [SerializeField] int _energy_cost;
    [SerializeField] float _cooldown;

    public void SetCooldown(float cooldown) {
        _cooldown = cooldown;
    }

    public void SetCanUse(CanUse can_use) {
        this.can_use = can_use;
    }

    public bool TryUse() {
        if (!on_cooldown && CanUseAbility() && character.TrySpendEnergy(_energy_cost)) {
            PutOnCooldown();
            UseAbility();
            return true;
        } else {
            return false;
        }
    }

    protected void PutOnCooldown() {
        time_off_cooldown = Time.time + _cooldown;
    }

    protected bool CanUseAbility() {
        return can_use != null ? can_use() : true;
    }

    protected abstract void UseAbility();
}
