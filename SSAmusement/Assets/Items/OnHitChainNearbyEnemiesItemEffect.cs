using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitChainNearbyEnemiesItemEffect : ItemEffect {
    
    [SerializeField] float spread_range;
    [SerializeField] Formula damage_formula;

    protected override void OnDrop() {
        item.owner.on_landed_hit += OnHit;
    }

    protected override void OnPickup() {
        item.owner.on_landed_hit += OnHit;
    }

    protected void OnHit(Character source, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        StrikeNearbyTargets(hit);
    }

    void StrikeNearbyTargets(Character hit) {
        List<Enemy> enemies = new List<Enemy>(RoomManager.instance.active.GetEnemies());
        foreach (Enemy e in enemies) {
            if (e.is_alive && Vector2.Distance(hit.transform.position, e.transform.position) < spread_range) {
                item.owner.DealDamage(damage_formula.GetValue(item.owner), e, false);
            }
        }
    }
}
