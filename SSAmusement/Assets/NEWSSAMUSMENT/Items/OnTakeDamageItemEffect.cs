using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnTakeDamageItemEffect : ItemEffect {
    public override void OnDrop(Item item) {
        item.owner.AddOnTakeDamage(OnTakeDamage);
    }

    public override void OnPickup(Item item) {
        item.owner.AddOnTakeDamage(OnTakeDamage);
    }

    protected abstract void OnTakeDamage(Character hit, float pre_damage, float post_damage, ICombatant source);
}
