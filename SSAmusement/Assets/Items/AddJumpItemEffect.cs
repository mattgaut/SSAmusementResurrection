using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddJumpItemEffect : ItemEffect {
    [SerializeField] int jumps;

    protected override void OnDrop() {
        item.owner.RemoveBonusJump(jumps);
    }

    protected override void OnFinalDrop() {
        OnDrop();
    }

    protected override void OnInitialPickup() {
        OnPickup();
    }

    protected override void OnPickup() {
        item.owner.AddBonusJump(jumps);
    }
}
