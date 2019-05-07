using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitItemEffect : ItemEffect {
    public override void OnDrop(Item item) {
        item.owner.RemoveOnHit(OnOwnerHitEnemy);
    }

    public override void OnPickup(Item item) {
        item.owner.AddOnHit(OnOwnerHitEnemy);
    }

    protected abstract void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, IDamageable hit);
}
