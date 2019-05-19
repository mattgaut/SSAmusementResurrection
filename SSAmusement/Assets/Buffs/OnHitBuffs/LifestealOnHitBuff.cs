using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestealOnHitBuff : OnHitBuff {
    [SerializeField] float lifesteal_modifier;
    public override void OnHitEffect(Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        hitter.RestoreHealth(post_mitigation_damage * lifesteal_modifier);
    }
}
