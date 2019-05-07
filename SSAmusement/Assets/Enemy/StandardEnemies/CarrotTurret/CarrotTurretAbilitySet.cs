using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotTurretAbilitySet : AbilitySet {

    public ActiveAbility shoot {
        get { return _shoot; }
    }

    [SerializeField] ActiveAbility _shoot;

    protected override void LoadSkills() {
        abilities.Add(_shoot);
    }
}
