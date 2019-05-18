using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAAEffect : ActiveAbilityEffect {

    [SerializeField] int health_restore_amount;
    [SerializeField] int energy_restore_amount;

    protected override void UseAbilityEffect(float input) {
        if (health_restore_amount > 0) character.RestoreHealth(health_restore_amount);
        if (energy_restore_amount > 0) character.RestoreEnergy(energy_restore_amount);
    }
}
