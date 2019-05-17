using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveChargeAbility : ActiveAbility {
    public override Type ability_type {
        get { return Type.ActiveCooldown; }
    }

    public override ActiveChargeAbility active_charge {
        get { return this; }
    }

}
