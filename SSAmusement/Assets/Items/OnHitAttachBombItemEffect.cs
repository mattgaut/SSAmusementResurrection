using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitAttachBombItemEffect : OnHitItemEffect {
    [SerializeField] StickyBomb bomb;
    [SerializeField] float chance_per_hit;
    [SerializeField] Formula damage_formula;
    [SerializeField] Vector2 knockback;

    RNG rng;

    protected override void OnPickup() {
        base.OnPickup();

        rng = new RNG();
    }

    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit) {
        if (rng.GetFloat() < chance_per_hit) {
            StickyBomb new_bomb = Instantiate(bomb);
            new_bomb.BeginBomb(character, hit, OnBombHit);
        }
    }

    void OnBombHit(Character hit, Attack hit_by) {
        item.owner.DealDamage(damage_formula.GetValue(item.owner), hit, false);
        item.owner.GiveKnockback(hit, knockback.MatchDirection(hit_by.transform.position, hit.transform.position));
    }
}
