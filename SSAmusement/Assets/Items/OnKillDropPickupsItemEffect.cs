using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKillDropPickupsItemEffect : OnKillItemEffect {

    [SerializeField] int enemies_per_drop;
    [SerializeField] Pickup to_drop;

    int count;

    protected override void OnKill(Character killer, Character killed) {
        count++;
        if (count >= enemies_per_drop) {
            killed.DropObject(to_drop.gameObject, true);
            count = 0;
        }
    }
}
