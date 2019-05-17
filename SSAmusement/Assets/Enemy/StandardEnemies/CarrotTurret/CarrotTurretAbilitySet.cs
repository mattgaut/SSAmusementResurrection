using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotTurretAbilitySet : AbilitySet {

    public ActiveCooldownAbility shoot {
        get { return _shoot; }
    }

    [SerializeField] ActiveCooldownAbility _shoot;

    protected override void LoadSkills() {
        abilities.Add(_shoot);
    }
}
