using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnTakeDamageItemEffect : ItemEffect {
    protected override void OnDrop() {
        item.owner.on_take_damage -= OnTakeDamage;
    }

    protected override void OnPickup() {
        item.owner.on_take_damage += OnTakeDamage;
    }

    protected abstract void OnTakeDamage(Character hit, float pre_damage, float post_damage, Character source);
}
