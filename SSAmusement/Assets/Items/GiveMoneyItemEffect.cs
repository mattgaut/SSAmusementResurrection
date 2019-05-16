using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMoneyItemEffect : ItemEffect {
    [SerializeField] int amount;

    protected override void OnDrop() {
        
    }

    protected override void OnPickup() {
        item.owner.inventory.AddCurrency(amount);
    }
}
