using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : Pickup {
    [SerializeField] float to_restore;

    protected override bool CanPickup(Player p) {
        return p.energy.current != p.energy.max;
    }

    protected override void PickupEffect(Player player) {
        player.RestoreEnergy(to_restore);
    }
}
