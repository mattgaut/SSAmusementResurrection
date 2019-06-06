using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBuffProjectileAAEffect : FireProjectileAAEffect {

    [SerializeField] BuffController buff;

    protected override void OnProjectileHit(Character hit, Attack hit_by) {
        base.OnProjectileHit(hit, hit_by);
        buff.ApplyBuff(hit);
    }
}
