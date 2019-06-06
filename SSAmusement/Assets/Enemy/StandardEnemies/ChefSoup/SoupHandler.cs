using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupHandler : MeleeAttackHandler {

    [SerializeField] BuffController soup_debuff;
    protected override void AttackOnHit(Character c) {
        base.AttackOnHit(c);
        soup_debuff.ApplyBuff(c);
    }

}
