using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBuffAAEffect : ActiveAbilityEffect {
    [SerializeField] BuffController buff;

    protected override void UseAbilityEffect(float input) {
        buff.ApplyBuff(character, character);
    }
}
