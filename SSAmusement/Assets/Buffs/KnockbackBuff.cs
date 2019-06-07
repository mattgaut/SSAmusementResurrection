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

    protected override void ApplyEffects(Character stat_entity, int id, IBuff buff) {        
        stat_entity.knockback_multiplier.AddModifier(modifier);
    }

    protected override void RemoveEffects(Character stat_entity, int id) {
        stat_entity.knockback_multiplier.RemoveModifier(modifier);
    }
}
