using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitExectuteItemEffect : OnHitItemEffect {
    [SerializeField][Range(0.0001f, 1f)] float execute_range;

    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit) {
        if (hit.health.percent <= execute_range) {
            hit.Execute(character);
        }
    }

}
