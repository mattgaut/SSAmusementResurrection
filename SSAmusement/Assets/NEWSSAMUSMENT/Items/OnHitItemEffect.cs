using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitItemEffect : ItemEffect {
    public override void OnDrop(Item item) {
        item.owner.RemoveOnHit(OnHit);
    }

    public override void OnPickup(Item item) {
        item.owner.AddOnHit(OnHit);
    }

    protected abstract void OnHit(Character character, float pre_damage, float post_damage, IDamageable hit);
}
