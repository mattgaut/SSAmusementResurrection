using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBuffOnHitBuff : OnHitBuff {

    [SerializeField] BuffController buff_to_apply;

    public override void OnHitEffect(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        buff_to_apply.ApplyBuff(hitter);
    }
}
