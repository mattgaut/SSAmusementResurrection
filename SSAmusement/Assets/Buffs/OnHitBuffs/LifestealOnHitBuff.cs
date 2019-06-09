using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestealOnHitBuff : OnHitBuff<OnHitBuffInfo> {
    [SerializeField] float lifesteal_modifier;

    protected override OnHitBuffInfo GetOnHitBuffInfo(IBuff buff) {
        return new OnHitBuffInfo(buff);
    }

    protected override void OnHitEffect(OnHitBuffInfo info, Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        hitter.RestoreHealth(post_mitigation_damage * lifesteal_modifier);
    }
}
