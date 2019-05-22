using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKillRestore : OnKillItemEffect {

    [SerializeField] float health_to_restore, energy_to_restore;

    protected override void OnKill(Character c, Character killed) {
        if (energy_to_restore > 0) c.RestoreEnergy(energy_to_restore);
        if (health_to_restore > 0) c.RestoreHealth(health_to_restore);
    }
}
