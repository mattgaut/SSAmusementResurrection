using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackBuff : BuffDefinition {
    [SerializeField] Stat.Modifier modifier;

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    protected override void Apply(Character stat_entity, int id) {        
        stat_entity.knockback_multiplier.AddModifier(modifier);
    }

    protected override void Remove(Character stat_entity, int id) {
        stat_entity.knockback_multiplier.RemoveModifier(modifier);
    }
}
