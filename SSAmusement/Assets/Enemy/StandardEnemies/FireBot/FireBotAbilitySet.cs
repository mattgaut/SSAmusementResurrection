using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBotAbilitySet : AbilitySet {
    public ActiveAbility projectile {
        get { return _projectile; }
    }
    public ActiveAbility field {
        get { return _field; }
    }

    [SerializeField] ActiveCooldownAbility _projectile;
    [SerializeField] ActiveCooldownAbility _field;

    protected override void LoadSkills() {
        abilities.Add(projectile);
        abilities.Add(field);
    }
}
