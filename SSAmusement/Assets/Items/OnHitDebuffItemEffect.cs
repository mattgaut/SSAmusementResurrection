using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitDebuffItemEffect : OnHitItemEffect {
    [SerializeField] BuffController to_apply;
    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit) {
        to_apply.ApplyBuff(hit);
    }
}
