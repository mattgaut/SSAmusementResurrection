using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitDebuffItemEffect : OnHitItemEffect {
    [SerializeField] BuffGroup to_apply;
    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit) {
        Character comb = hit as Character;
        if (comb != null) to_apply.GetIBuffInstance().Apply(comb);
    }
}
