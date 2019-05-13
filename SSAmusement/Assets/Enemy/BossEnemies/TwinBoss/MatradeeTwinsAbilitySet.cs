using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatradeeTwinsAbilitySet : AbilitySet {

    public CycloneAbility cyclone {
        get { return _cyclone; }
    }

    public ActiveAbility throw_knives {
        get { return _throw_knives; }
    }

    public ActiveAbility throw_plates {
        get { return _throw_plates; }
    }

    [SerializeField] CycloneAbility _cyclone;
    [SerializeField] ActiveAbility _throw_plates;
    [SerializeField] ActiveAbility _throw_knives;

    protected override void LoadSkills() {
        abilities.Add(_throw_knives);
        abilities.Add(_throw_plates);
        abilities.Add(_cyclone);
    }
}
