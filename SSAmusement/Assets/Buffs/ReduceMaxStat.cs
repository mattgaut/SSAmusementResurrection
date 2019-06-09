using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceMaxStatInfo : BuffInfo {
    public Stat.Modifier modifier { get; private set; }
    public ReduceMaxStatInfo(IBuff info, Stat.Modifier modifier) : base(info) {
        this.modifier = modifier;
    }
}

public class ReduceMaxStat : BuffDefinition<ReduceMaxStatInfo> {

    [SerializeField] float percent_reduction;
    public override BuffType type {
        get { return BuffType.stat; }
    }

    protected override void ApplyEffects(Character character, ReduceMaxStatInfo info, IBuff buff) {
        character.health.AddModifier(info.modifier);
        info.modifier.SetFlat(-(percent_reduction * character.health.flat_modded_value));
    }

    protected override void RemoveEffects(Character character, ReduceMaxStatInfo info) {
        character.health.RemoveModifier(info.modifier);
    }

    protected override ReduceMaxStatInfo GetBuffInfo(IBuff buff) {
        return new ReduceMaxStatInfo(buff, new Stat.Modifier());
    }

}
