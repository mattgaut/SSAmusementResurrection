using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAbilitySet : AbilitySet {

    [SerializeField] ActiveCooldownAbility basic_attack;
    [SerializeField] ActiveCooldownAbility counter;
    [SerializeField] ActiveCooldownAbility dash;

    bool can_use_basic_attack {
        get {
            return !(basic_attack.is_using_ability || counter.is_using_ability);
        }
    }
    bool can_use_counter {
        get {
            return !(basic_attack.is_using_ability || counter.is_using_ability);
        }
    }

    protected override void LoadSkills() {
        abilities.Add(basic_attack);
        abilities.Add(counter);
        abilities.Add(dash);

        basic_attack.SetCanUse(() => can_use_basic_attack);
        counter.SetCanUse(() => can_use_counter);
    }
}
