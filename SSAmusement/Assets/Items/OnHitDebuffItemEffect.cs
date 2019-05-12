using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitDebuffItemEffect : OnHitItemEffect {
    [SerializeField] BuffGroup to_apply;
    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, IDamageable hit) {
        ICombatant comb = hit as ICombatant;
        if (comb != null) to_apply.Apply(comb);
    }
}
