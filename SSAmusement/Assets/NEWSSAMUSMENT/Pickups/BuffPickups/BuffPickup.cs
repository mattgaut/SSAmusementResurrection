using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPickup : Pickup {

    [SerializeField] BuffGroup buff;

    protected override void PickupEffect(Player player) {
        buff.Apply(player);
    }
}
