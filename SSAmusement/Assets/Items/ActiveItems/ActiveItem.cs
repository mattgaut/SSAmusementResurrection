using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItem : Item {

    public override Type item_type {
        get { return Type.active; }
    }

    public ActiveChargeAbility active_ability {
        get { return ability; }
    }

    [SerializeField] ActiveChargeAbility ability;

    public override void OnPickup(Player p) {
        base.OnPickup(p);

        ability.SetCharacter(p);
    }
    public override void OnDrop(Player p) {
        base.OnDrop(p);

        ability.SetCharacter(null);
    }
}
