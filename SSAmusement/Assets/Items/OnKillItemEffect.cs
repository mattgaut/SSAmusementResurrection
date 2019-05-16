using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnKillItemEffect : ItemEffect {

    protected override void OnDrop() {
        item.owner.on_kill -= OnKill;
    }

    protected override void OnPickup() {
        item.owner.on_kill += OnKill;
    }

    protected abstract void OnKill(Character c, ICombatant killed);
}
