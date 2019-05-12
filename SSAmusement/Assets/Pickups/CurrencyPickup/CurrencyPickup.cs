using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPickup : Pickup {
    [SerializeField] int amount_to_gain;

    protected override void PickupEffect(Player player) {
        player.inventory.AddCurrency(amount_to_gain);
    }
}
