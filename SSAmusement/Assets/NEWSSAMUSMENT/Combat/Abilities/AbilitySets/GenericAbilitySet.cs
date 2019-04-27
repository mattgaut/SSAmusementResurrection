using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAbilitySet : AbilitySet {
    [SerializeField] Ability[] generic_abilities;

    protected override void LoadSkills() {
        abilities.AddRange(generic_abilities);
    }
}
