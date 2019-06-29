using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitItemEffect : ItemEffect {
    protected override void OnDrop() {
        item.owner.on_landed_hit -= OnOwnerHitEnemy;
    }

    protected override void OnPickup() {
        item.owner.on_landed_hit += OnOwnerHitEnemy;
    }

    protected abstract void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit);
}
