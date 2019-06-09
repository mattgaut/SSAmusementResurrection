using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayingStatBuffInfo : StatBuffInfo {
    public Coroutine decay_routine { get; set; }
    public DecayingStatBuffInfo(IBuff buff, Stat.Modifier modifier) : base(buff, modifier) {

    }
}

public class DecayingStatBuff : StatBuff<DecayingStatBuffInfo> {

    protected override void ApplyEffects(Character character, DecayingStatBuffInfo info, IBuff buff) {
        base.ApplyEffects(character, info, buff);

        if (buff.length > 0) {
            info.decay_routine = StartCoroutine(DecayModifier(info.modifier, buff));
        }
    }

    protected override DecayingStatBuffInfo GetBuffInfo(IBuff buff) {
        return new DecayingStatBuffInfo(buff, new Stat.Modifier(modifier));
    }

    IEnumerator DecayModifier(Stat.Modifier to_decay, IBuff buff_info) {
        float original_flat = to_decay.flat, original_multi = to_decay.multi;

        while (buff_info.is_active) {
            yield return new WaitForFixedUpdate();
            float percent = (buff_info.remaining_time / buff_info.length) + (buff_info.stack_count - 1);
            if (original_multi != 0) {
                to_decay.Set(original_flat * percent, original_multi * percent);
            } else {
                to_decay.SetFlat(original_flat * percent);
            }
        }

        to_decay.Set(0, 0);
    }
}
