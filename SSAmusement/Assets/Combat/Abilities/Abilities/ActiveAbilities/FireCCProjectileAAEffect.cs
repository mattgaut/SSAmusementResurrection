using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCCProjectileAAEffect : FireProjectileAAEffect {

    [SerializeField] List<CrowdControl.Type> crowd_control_effect;
    [SerializeField] float duration;

    protected override void OnProjectileHit(Character hit, Attack hit_by) {
        base.OnProjectileHit(hit, hit_by);
        foreach (CrowdControl.Type type in crowd_control_effect) {
            hit.crowd_control_effects.ApplyCC(type, duration, character);
        }
    }
}
