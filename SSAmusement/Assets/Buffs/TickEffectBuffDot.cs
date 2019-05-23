using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickEffectBuffDot : TickEffectBuff {

    [SerializeField] float damage;
    protected override void TickEffect(Character stat_entity) {
        stat_entity.TakeDamage(damage, stat_entity, true);
    }
}
