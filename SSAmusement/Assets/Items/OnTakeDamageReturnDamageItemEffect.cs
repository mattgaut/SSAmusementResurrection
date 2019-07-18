using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTakeDamageReturnDamageItemEffect : OnTakeDamageItemEffect {
    [SerializeField] bool is_pre_mitigation_damage;
    [SerializeField] float damage_multtiplier;
    [SerializeField] float flat_damage;

    protected override void OnTakeDamage(Character hit, float pre_damage, float post_damage, Character source) {
        hit.DealDamage(((is_pre_mitigation_damage ? pre_damage : post_damage) * damage_multtiplier * item.stack_count) + flat_damage, source, false);
    }
}
