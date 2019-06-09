using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickEffectBuffDot : TickEffectBuff<TickEffectBuffInfo> {

    [SerializeField] float damage;

    protected override TickEffectBuffInfo GetBuffInfo(IBuff buff) {
        return new TickEffectBuffInfo(buff);
    }

    protected override void TickEffect(TickEffectBuffInfo info) {
        info.buff.buffed.TakeDamage(damage, info.buff.source, true);
    }
}
