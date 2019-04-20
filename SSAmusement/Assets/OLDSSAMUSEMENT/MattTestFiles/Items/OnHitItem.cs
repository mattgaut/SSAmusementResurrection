using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitItem : Item {
    protected override void OnDrop() {
        owner.RemoveOnHit(OnHit);
    }

    protected override void OnPickup() {
        owner.AddOnHit(OnHit);
    }

    protected abstract void OnHit(Character character, float pre_damage, float post_damage, IDamageable hit);
}
