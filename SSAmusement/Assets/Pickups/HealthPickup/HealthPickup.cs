using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthPickup : Pickup {

    [SerializeField] float health_restored;

    protected override bool CanPickup(Player p) {
        return p.health.current != p.health.max;
    }

    protected override void PickupEffect(Player player) {
        player.RestoreHealth(health_restored);
    }

}
