using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAbilitySet : AbilitySet {

    [SerializeField] MeleeAttackAbility basic_attack;
    [SerializeField] SwordsmanCounterAbility counter;
    [SerializeField] DashAbility dash;

    bool can_use_basic_attack {
        get {
            return !(basic_attack.using_ability || counter.using_ability);
        }
    }
    bool can_use_counter {
        get {
            return !(basic_attack.using_ability || counter.using_ability);
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
