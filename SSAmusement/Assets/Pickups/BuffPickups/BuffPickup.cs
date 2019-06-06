using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPickup : Pickup {

    [SerializeField] BuffController buff;

    protected override void PickupEffect(Player player) {
        buff.ApplyBuff(player);
    }
}
