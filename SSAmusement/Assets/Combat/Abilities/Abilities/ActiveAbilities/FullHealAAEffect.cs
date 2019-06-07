using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealAAEffect : ActiveAbilityEffect {

    [SerializeField] bool energy, health;

    protected override void UseAbilityEffect(float input) {
        if (health) character.RestoreHealth(character.health.max);
        if (energy) character.RestoreEnergy(character.energy.max);
    }
}
