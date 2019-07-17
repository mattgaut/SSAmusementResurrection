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

    public bool is_on_cooldown { get { return charges < max_charges; } }

    public override bool is_available {
        get { return base.is_available && charges > 0; }
    }

    public float time_cooldown_ends { get; private set; }
    public float time_until_cooldown_ends { get { return time_cooldown_ends - Time.time; } }
    
    public int max_charges { get { return _max_charges + bonus_charges; } }
    public int charges { get; private set; }
    public int bonus_charges { get; private set; }

    [SerializeField][FormerlySerializedAs("_cooldown")] float base_cooldown;
    [SerializeField] CooldownType cooldown_reduction_type;
    [SerializeField] int _max_charges = 1;

    float last_speed = 0f;

    Coroutine cooldown_routine;

    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        charges = max_charges;
    }

    public void SetCooldown(float cooldown) {
        base_cooldown = cooldown;
    }

    public void RefundCooldown(float refund) {
        time_cooldown_ends -= refund;
    }

    public void RefundPercent(float refund) {
        time_cooldown_ends -= time_until_cooldown_ends * refund;
    }

    public void ModifyBonusCharges(int count) {        
        bonus_charges += count;
        if (bonus_charges < 0) {
            bonus_charges = 0;
        }
        if (count > 0) {
            charges += count;
        }
        if (charges > max_charges) {
            charges = max_charges;
        }
    }

    protected override void OnAbilityUsed() {
        charges--;
        if (cooldown_routine == null) {
            cooldown_routine = character.StartCoroutine(Cooldown());
        }
        if (cooldown_reduction_type == CooldownType.AttackSpeed) character.animator.SetFloat("AttackSpeed", character.stats.attack_speed);
    }

    protected override bool TryPayCost(int cost) {
        return character.TrySpendEnergy(cost);
    }

    float ReducedCooldown() {
        if (cooldown_reduction_type == CooldownType.Irreduceable) {
            return base_cooldown;
        } else if (cooldown_reduction_type == CooldownType.CooldownReduction) {
            return base_cooldown * (1f - character.stats.cooldown_reduction);
        } else if (cooldown_reduction_type == CooldownType.AttackSpeed) {
            return base_cooldown * (1f / character.stats.attack_speed);
        } else {
            return base_cooldown;
        }
    }

    IEnumerator Cooldown() {
        time_cooldown_ends = Time.time;
        while (charges < max_charges) {
            time_cooldown_ends += ReducedCooldown();
            while (time_until_cooldown_ends > 0f) {
                yield return null;
            }
            ReturnCharge();
        }
        cooldown_routine = null;
    }

    void ReturnCharge() {
        charges++;
    }
}
