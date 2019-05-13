using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddJumpItemEffect : ItemEffect {
    [SerializeField] int jumps;

    public override void OnDrop(Item item) {
        item.owner.RemoveBonusJump(jumps);
    }

    public override void OnPickup(Item item) {
        item.owner.AddBonusJump(jumps);
    }
}
