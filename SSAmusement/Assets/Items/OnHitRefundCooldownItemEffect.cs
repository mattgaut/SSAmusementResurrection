using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitRefundCooldownItemEffect : OnHitItemEffect {

    List<ActiveCooldownAbility> possible_abilities;

    [SerializeField] float refund;
    [SerializeField] bool is_percent;

    protected override void OnPickup() {
        base.OnPickup();

        possible_abilities = new List<ActiveCooldownAbility>();
        foreach (Ability a in item.owner.abilities.GetAbilities()) {
            if (a.active_cooldown != null) {
                possible_abilities.Add(a.active_cooldown);
            }
        }
    }

    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit) {
        if (possible_abilities.Count > 0) {
            possible_abilities.Shuffle();
            foreach (ActiveCooldownAbility a in possible_abilities) {
                if (a.is_on_cooldown) {
                    if (is_percent) {
                        a.RefundPercent(refund);
                    } else {
                        a.RefundCooldown(refund);
                    }
                    break;
                }
            }
        }
    }
}
