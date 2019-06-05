using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupHandler : MeleeAttackHandler {

    [SerializeField] BuffGroup soup_debuff;
    protected override void AttackOnHit(Character c) {
        base.AttackOnHit(c);
        Character comb = c as Character;
        if (comb != null) soup_debuff.GetIBuffInstance().Apply(comb);
    }

}
