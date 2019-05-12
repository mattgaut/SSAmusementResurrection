using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatradeeTwinsAbilitySet : AbilitySet {

    public CycloneAbility cyclone {
        get { return _cyclone; }
    }

    [SerializeField] CycloneAbility _cyclone;

    protected override void LoadSkills() {

        abilities.Add(_cyclone);
    }
}
