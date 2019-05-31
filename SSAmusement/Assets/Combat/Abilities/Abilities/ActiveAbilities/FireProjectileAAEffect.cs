using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectileAAEffect : ActiveAbilityEffect {

    [SerializeField] Projectile to_fire;
    [SerializeField] Vector2 knockback;
    [SerializeField] Formula formula;

    [SerializeField] Transform projectile_spawn_transform;

    protected override void UseAbilityEffect(float input) {
        FireProjectile();
    }

    protected void FireProjectile() {
        Projectile new_projectile = Instantiate(to_fire);
        new_projectile.SetSource(character);
        new_projectile.SetOnHit(OnProjectileHit);

        new_projectile.transform.position = projectile_spawn_transform.position;
        new_projectile.transform.rotation = projectile_spawn_transform.rotation;
    }

    protected virtual void OnProjectileHit(Character hit, Attack hit_by) {
        if (knockback != Vector2.zero) {
            hit.TakeKnockback(character, knockback.MatchDirection(hit_by.transform.position, hit.transform.position));
        }
        character.DealDamage(formula.GetValue(character), hit, true);
    }
}
