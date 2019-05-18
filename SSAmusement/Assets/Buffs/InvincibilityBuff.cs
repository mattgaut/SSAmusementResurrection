using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityBuff : BuffDefinition {
    public override BuffType type {
        get { return BuffType.invincibility; }
    }

    Dictionary<ICombatant, List<int>> lock_values;

    public override void Apply(ICombatant stat_entity) {
        if (!lock_values.ContainsKey(stat_entity)) {
            lock_values.Add(stat_entity, new List<int>());
        }
        lock_values[stat_entity].Add(stat_entity.LockInvincibility());
    }

    public override void Remove(ICombatant stat_entity) {
        if (lock_values[stat_entity].Count > 0) {
            stat_entity.UnlockInvincibility(lock_values[stat_entity][0]);
            lock_values[stat_entity].RemoveAt(0);
        }
    }

    protected override void Init() {
        lock_values = new Dictionary<ICombatant, List<int>>();
    }
}
