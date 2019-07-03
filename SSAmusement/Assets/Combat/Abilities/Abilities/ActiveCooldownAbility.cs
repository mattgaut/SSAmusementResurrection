using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum CooldownType { Irreduceable, AttackSpeed, CooldownReduction }

[Serializable]
public sealed class ActiveCooldownAbility : ActiveAbility {

    public override Type ability_type {
        get { return Type.ActiveCooldown; }
    }

    public override ActiveCooldownAbility active_cooldown {
        get { return this; }
    }

    public float cooldown { get { return ReducedCooldown(); } }

    public bool is_on_cooldown { get { return time_cooldown_ends > Time.time; } }

    public override bool is_available {
        get { return base.is_available && !is_on_cooldown; }
    }

    public float time_cooldown_ends { get; private set; }
    public float time_until_cooldown_ends { get { return time_cooldown_ends - Time.time; } }

    [SerializeField][FormerlySerializedAs("_cooldown")] float base_cooldown;
    [SerializeField] CooldownType cooldown_reduction_type; 

    public void SetCooldown(float cooldown) {
        base_cooldown = cooldown;
    }

    protected override void OnAbilityUsed() {
        time_cooldown_ends = Time.time + ReducedCooldown();
    }

    protected override bool TryPayCost(int cost) {
        return character.TrySpendEnergy(cost);
    }

    float ReducedCooldown() {
        if (cooldown_reduction_type == CooldownType.Irreduceable) {
            return base_cooldown;
        } else if (cooldown_reduction_type == CooldownType.CooldownReduction) {
            return base_cooldown * (100f - character.stats.cooldown_reduction);
        } else if (cooldown_reduction_type == CooldownType.AttackSpeed) {
            return base_cooldown * (1f / character.stats.attack_speed);
        } else {
            return base_cooldown;
        }
    }
}
