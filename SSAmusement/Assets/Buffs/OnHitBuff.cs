using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitBuff : BuffDefinition {
    public override BuffType type {
        get {
            return BuffType.on_hit;
        }
    }

    public override void Apply(Character stat_entity) {
        stat_entity.character.on_hit += OnHitEffect;
    }

    public override void Remove(Character stat_entity) {
        stat_entity.character.on_hit -= OnHitEffect;
    }

    public abstract void OnHitEffect(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit);
}
