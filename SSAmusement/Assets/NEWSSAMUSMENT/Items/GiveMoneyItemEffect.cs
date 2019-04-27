using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMoneyItemEffect : ItemEffect {
    [SerializeField] int amount;

    public override void OnDrop(Item i) {
        
    }

    public override void OnPickup(Item i) {
        i.owner.inventory.AddCurrency(amount);
    }
}
