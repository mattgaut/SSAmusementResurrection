using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActiveCooldownAbility : ActiveAbility {

    public override Type ability_type {
        get { return Type.ActiveCooldown; }
    }

    public override ActiveCooldownAbility active_cooldown {
        get { return this; }
    }

    public float cooldown { get { return _cooldown; } }

    public bool is_on_cooldown { get { return time_cooldown_ends > Time.time; } }

    public override bool is_available {
        get { return base.is_available && !is_on_cooldown; }
    }

    public float time_cooldown_ends { get; private set; }
    public float time_until_cooldown_ends { get { return time_cooldown_ends - Time.time; } }

    [SerializeField] float _cooldown;

    public void SetCooldown(float cooldown) {
        _cooldown = cooldown;
    }

    protected override void OnAbilityUsed() {
        time_cooldown_ends = Time.time + _cooldown;
        on_ability_used.Invoke();
    }

    protected override bool TryPayCost(int cost) {
        return character.TrySpendEnergy(cost);
    }
}
