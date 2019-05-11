using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnTakeDamageItemEffect : ItemEffect {
    public override void OnDrop(Item item) {
        item.owner.on_take_damage -= OnTakeDamage;
    }

    public override void OnPickup(Item item) {
        item.owner.on_take_damage += OnTakeDamage;
    }

    protected abstract void OnTakeDamage(Character hit, float pre_damage, float post_damage, ICombatant source);
}
