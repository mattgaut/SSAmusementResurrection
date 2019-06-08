using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeOnHitAAEffect : MeleeAAEffect {

    [SerializeField] BuffController on_hit_buff;

    protected override void AttackOnHit(Character d, Attack hit_by) {
        base.AttackOnHit(d, hit_by);
        on_hit_buff.ApplyBuff(d);
    }
}
