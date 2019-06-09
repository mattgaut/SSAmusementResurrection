using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackBuffInfo : BuffInfo {
    public Stat.Modifier modifier { get; private set; }
    public KnockbackBuffInfo(IBuff buff, Stat.Modifier modifier) : base(buff) {
        this.modifier = modifier;
    }
}

public class KnockbackBuff : BuffDefinition<KnockbackBuffInfo> {
    [SerializeField] Stat.Modifier modifier;

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    protected override void ApplyEffects(Character character, KnockbackBuffInfo info, IBuff buff) {
        character.knockback_multiplier.AddModifier(modifier);
    }

    protected override void RemoveEffects(Character character, KnockbackBuffInfo info) {
        character.knockback_multiplier.RemoveModifier(modifier);
    }

    protected override void RecalculateEffects(KnockbackBuffInfo info, IBuff buff) {
        info.modifier.Set(modifier * buff.stack_count);
    }

    protected override KnockbackBuffInfo GetBuffInfo(IBuff buff) {
        return new KnockbackBuffInfo(buff, new Stat.Modifier(modifier));
    }
}
