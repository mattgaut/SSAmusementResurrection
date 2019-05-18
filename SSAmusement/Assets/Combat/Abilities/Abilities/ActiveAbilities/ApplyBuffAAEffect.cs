using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBuffAAEffect : ActiveAbilityEffect {
    [SerializeField] BuffGroup buff;

    protected override void UseAbilityEffect(float input) {
        buff.Apply(character);
    }
}
