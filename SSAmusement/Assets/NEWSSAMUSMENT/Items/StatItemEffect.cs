using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItemEffect : ItemEffect {

    [SerializeField] List<StatBuff> buffs_to_apply;

    public override void OnDrop(Item item) {
        foreach (StatBuff b in buffs_to_apply) {
            b.RemoveFrom(item.owner);
        }
    }

    public override void OnPickup(Item item) {
        foreach (StatBuff b in buffs_to_apply) {
            b.ApplyTo(item.owner);
        }
    }
}
