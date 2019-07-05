using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedBuff : BuffDefinition<StatBuffInfo> {

    [SerializeField] protected Stat.Modifier modifier;

    public override BuffType type {
        get { return BuffType.stat; }
    }

    protected override void ApplyEffects(Character character, StatBuffInfo info, IBuff buff) {
        character.stats.attack_speed.AddModifier(modifier);
    }

    protected override StatBuffInfo GetBuffInfo(IBuff buff) {
        return new StatBuffInfo(buff, new Stat.Modifier(modifier));
    }

    protected override void RecalculateEffects(StatBuffInfo info, IBuff buff) {
        info.modifier.Set(modifier * buff.stack_count);
    }
    protected override void RemoveEffects(Character character, StatBuffInfo info) {
        character.stats.attack_speed.RemoveModifier(modifier);
    }
}
