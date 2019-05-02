using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossKeyPickup : Pickup {

    protected override void PickupEffect(Player player) {
        player.inventory.AddKeycard(1);
    }

}
