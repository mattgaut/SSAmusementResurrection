using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackBuff : BuffDefinition, IStatBuff {
    [SerializeField] float _multi;

    public float flat {
        get { return 0; }
    }
    public float multi {
        get { return _multi; }
    }

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    public override void Apply(ICombatant stat_entity) {        
        stat_entity.knockback_multiplier.AddBuff(this);
    }

    public override void Remove(ICombatant stat_entity) {
        stat_entity.knockback_multiplier.RemoveBuff(this);
    }
}
