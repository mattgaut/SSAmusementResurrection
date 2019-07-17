using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnKillItemEffect : ItemEffect {

    protected override void OnFinalDrop() {
        item.owner.on_kill -= OnKill;
    }

    protected override void OnInitialPickup() {
        item.owner.on_kill += OnKill;
    }

    protected abstract void OnKill(Character killer, Character killed);
}
