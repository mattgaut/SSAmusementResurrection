using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSpendEnergyLaunchProjectileItemEffect : OnSpendEnergyItemEffect {
    [SerializeField] HomingProjectile projectile;
    [SerializeField] Vector2 knockback;

    [SerializeField] TargetCollector target_collector;

    protected override void OnSpendEnergy(Character source, float spent) {
        HomingProjectile new_projectile = Instantiate(projectile);
        new_projectile.transform.position = source.stats.center_mass.position;

        new_projectile.LaunchTowardsTarget(Vector2.up);

        new_projectile.SetSource(source);
        new_projectile.SetOnHit((hit, hit_by) => OnHit(hit, hit_by, spent));
        Character target = target_collector.GetRandomTarget();
        new_projectile.SetTarget(target);
    }

    void OnHit(Character hit, Attack hit_by, float energy_spent) {
        hit_by.source.DealDamage(energy_spent * item.stack_count, hit, true);
        hit_by.source.GiveKnockback(hit, knockback.MatchDirection(hit_by.transform.position, hit.transform.position));
    }
}
