using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatradeeTwinsAbilitySet : AbilitySet {

    public ActiveCooldownAbility cyclone {
        get { return _cyclone; }
    }

    public bool is_cyclone_winding_up {
        get { return cyclone_ability_effect.is_winding_up; }
    }

    public ActiveCooldownAbility throw_knives {
        get { return _throw_knives; }
    }

    public ActiveCooldownAbility throw_plates {
        get { return _throw_plates; }
    }

    [SerializeField] ActiveCooldownAbility _cyclone;
    [SerializeField] ActiveCooldownAbility _throw_plates;
    [SerializeField] ActiveCooldownAbility _throw_knives;

    [SerializeField] CycloneAAEffect cyclone_ability_effect;

    protected override void LoadSkills() {
        abilities.Add(_throw_knives);
        abilities.Add(_throw_plates);
        abilities.Add(_cyclone);
    }
}
