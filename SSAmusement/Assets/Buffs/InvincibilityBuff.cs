using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityBuff : BuffDefinition {
    public override BuffType type {
        get { return BuffType.invincibility; }
    }

    Dictionary<Character, List<int>> lock_values;

    protected override void ApplyEffects(Character stat_entity, int id) {
        if (!lock_values.ContainsKey(stat_entity)) {
            lock_values.Add(stat_entity, new List<int>());
        }
        lock_values[stat_entity].Add(stat_entity.LockInvincibility());
    }

    protected override void RemoveEffects(Character stat_entity, int id) {
        if (lock_values[stat_entity].Count > 0) {
            stat_entity.UnlockInvincibility(lock_values[stat_entity][0]);
            lock_values[stat_entity].RemoveAt(0);
        }
    }

    protected override void Init() {
        lock_values = new Dictionary<Character, List<int>>();
    }
}
