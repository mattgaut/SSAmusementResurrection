using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitBuff : BuffDefinition {
    public override BuffType type {
        get {
            return BuffType.on_hit;
        }
    }

    protected override void ApplyEffects(Character stat_entity, int id) {
        stat_entity.character.on_hit += OnHitEffect;
    }

    protected override void RemoveEffects(Character stat_entity, int id) {
        stat_entity.character.on_hit -= OnHitEffect;
    }

    public abstract void OnHitEffect(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit);
}
