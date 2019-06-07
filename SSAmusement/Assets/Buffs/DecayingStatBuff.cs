using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayingStatBuff : StatBuff {

    Dictionary<int, Stat.Modifier> modifiers;

    protected override void Init() {
        base.Init();
        modifiers = new Dictionary<int, Stat.Modifier>();
    }

    protected override void ApplyEffects(Character character, int id, IBuff buff) {
        Stat.Modifier new_modifier = new Stat.Modifier(modifier);

        AddModifier(character, new_modifier);

        modifiers.Add(id, new_modifier);

        if (buff.length > 0) {
            StartCoroutine(DecayModifier(new_modifier, buff));
        }
    }

    protected override void RemoveEffects(Character character, int id) {
        if (modifiers.ContainsKey(id)) {
            RemoveModifier(character, modifiers[id]);
            modifiers.Remove(id);
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
}
