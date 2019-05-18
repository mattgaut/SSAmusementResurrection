using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ActiveChargeAbility : ActiveAbility {
    public override Type ability_type {
        get { return Type.ActiveCharge; }
    }

    public override ActiveChargeAbility active_charge {
        get { return this; }
    }

    public override bool is_available {
        get { return charges >= cost; }
    }

    public int charges {
        get; private set;
    }

    public event Action<int, int> on_charge_changed;

    public override void SetCharacter(Character character) {
        if (this.character != null) {
            this.character.on_kill -= OnKill;
        }
        if (character != null) {
            character.on_kill += OnKill;
        }
        base.SetCharacter(character);
    }

    protected override void OnAbilityUsed() {

    }

    protected override bool TryPayCost(int cost) {
        if (cost <= charges) {
            charges -= cost;
            on_charge_changed?.Invoke(charges + cost, charges);
            return true;
        }
        return false;
    }

    void AddCharge(int to_add) {
        if (charges < cost) {
            int old_charges = charges;
            charges += to_add;
            charges = Mathf.Min(charges, cost);

            on_charge_changed?.Invoke(old_charges, charges);
        }
    }

    void OnKill(Character a, ICombatant b) {
        AddCharge(1);
    }
}
