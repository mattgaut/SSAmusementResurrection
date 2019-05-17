using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ActiveChargeAbility : ActiveAbility {
    public override Type ability_type {
        get { return Type.ActiveCooldown; }
    }

    public override ActiveChargeAbility active_charge {
        get { return this; }
    }

    protected override void OnAbilityUsed() {
        throw new System.NotImplementedException();
    }

    protected override bool TryPayCost(int cost) {
        throw new System.NotImplementedException();
    }
}
