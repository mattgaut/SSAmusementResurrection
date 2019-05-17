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

    public override void SetCharacter(Character character) {
        if (this.character != null) {
            this.character.on_kill -= (a, b) => { if (charges < cost) charges += 1; };
        }
        if (character != null) {
            character.on_kill += (a, b) => { if (charges < cost) charges += 1; };
        }
        base.SetCharacter(character);
    }

    protected override void OnAbilityUsed() {

    }

    protected override bool TryPayCost(int cost) {
        if (cost <= charges) {
            charges -= cost;
            return true;
        }
        return false;
    }
}
