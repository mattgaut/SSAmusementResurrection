using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMoneyItemEffect : ItemEffect {
    [SerializeField] int amount;

    protected override void OnFinalDrop() {
    }

    protected override void OnInitialPickup() {
        OnPickup();
    }

    protected override void OnPickup() {
        item.owner.inventory.AddCurrency(amount);
    }
}
