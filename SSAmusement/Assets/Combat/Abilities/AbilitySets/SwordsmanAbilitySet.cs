using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAbilitySet : AbilitySet {

    [SerializeField] ActiveCooldownAbility basic_attack;
    [SerializeField] ActiveCooldownAbility counter;
    [SerializeField] ActiveCooldownAbility dash;
    [SerializeField] ActiveCooldownAbility projectile;

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
    bool can_use_projectile {
        get {
            return !(basic_attack.is_using_ability || counter.is_using_ability);
        }
    }

    protected override void LoadSkills() {
        abilities.Add(basic_attack);
        abilities.Add(counter);
        abilities.Add(dash);
        abilities.Add(projectile);

        basic_attack.SetCanUse(() => can_use_basic_attack);
        counter.SetCanUse(() => can_use_counter);
        projectile.SetCanUse(() => can_use_projectile);
    }
}
