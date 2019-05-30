﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKillLaunchProjectile : OnKillItemEffect {

    [SerializeField] HomingProjectile projectile;
    [SerializeField] Vector2 knockback;

    [SerializeField] TargetCollector target_collector;

    protected override void OnKill(Character killer, Character killed) {
        HomingProjectile new_projectile = Instantiate(projectile);
        new_projectile.transform.position = killer.char_definition.center_mass.position;

        new_projectile.LaunchTowardsTarget(Vector2.up);

        new_projectile.SetSource(killer);
        new_projectile.SetOnHit(OnHit);
        new_projectile.SetTarget(target_collector.GetRandomTarget());
    }

    void OnHit(Character hit, Attack hit_by) {
        hit_by.source.DealDamage(hit_by.source.power, hit, true);
        hit_by.source.GiveKnockback(hit, knockback.ApplyDirectionalKnockback(hit_by.transform.position, hit.transform.position));
    }
}