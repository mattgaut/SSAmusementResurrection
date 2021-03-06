﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoClocheAbilitySet : AbilitySet {

    public ActiveCooldownAbility dive_bomb {
        get { return _dive_bomb; }
    }
    public ActiveCooldownAbility sweep {
        get { return _sweep; }
    }

    [SerializeField] ActiveCooldownAbility _dive_bomb;
    [SerializeField] ActiveCooldownAbility _sweep;

    bool can_use_ability {
        get {
            return !(dive_bomb.is_using_ability || sweep.is_using_ability || character.is_knocked_back);
        }
    }

    protected override void LoadSkills() {
        abilities.Add(dive_bomb);
        abilities.Add(sweep);

        dive_bomb.SetCanUse(() => can_use_ability);
        sweep.SetCanUse(() => can_use_ability);
    }
}
