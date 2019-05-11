using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnKillItemEffect : ItemEffect {

    public override void OnDrop(Item item) {
        item.owner.on_kill -= OnKill;
    }

    public override void OnPickup(Item item) {
        item.owner.on_kill += OnKill;
    }

    protected abstract void OnKill(Character c, ICombatant killed);
}
