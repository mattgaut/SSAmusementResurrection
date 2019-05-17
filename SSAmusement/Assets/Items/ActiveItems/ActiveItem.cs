using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItem : Item {

    public override Type item_type {
        get { return Type.active; }
    }

    public override void OnPickup(Player p) {
        base.OnDrop(p);
    }
    public override void OnDrop(Player p) {
        base.OnDrop(p);
    }
}
