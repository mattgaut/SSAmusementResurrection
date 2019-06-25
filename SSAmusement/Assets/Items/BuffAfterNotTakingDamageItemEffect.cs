using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAfterNotTakingDamageItemEffect : OnTakeDamageItemEffect {
    [SerializeField] float length_before_buff;
    [SerializeField] BuffController buff;

    float time_since_hit = 0f;
    int current_buff = 0;

    private void Update() {
        if (item.owner != null) {
            time_since_hit += GameManager.GetDeltaTime(item.owner.team);

            if (time_since_hit > length_before_buff && !buff.IsApplied(current_buff)) {
                current_buff = buff.ApplyBuff(item.owner, item.owner, true);
            }
        }
    }

    protected override void OnTakeDamage(Character hit, float pre_damage, float post_damage, Character source) {
        time_since_hit = 0f;
        buff?.RemoveBuff(current_buff);
    }
}