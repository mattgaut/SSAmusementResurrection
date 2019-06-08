using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayingStatBuff : StatBuff {

    protected override void ApplyEffects(Character character, int id, IBuff buff) {
        base.ApplyEffects(character, id, buff);

        if (buff.length > 0) {
            StartCoroutine(DecayModifier(modifiers[id], buff));
        }
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

    protected override void RecalculateEffects(int id, IBuff info) {
    }
}
