﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCarrotAbility : ActiveAbility {

    [SerializeField] Transform projectile_spawn_point;
    [SerializeField] Projectile projectile_prefab;

    protected override void UseAbility(float input) {
        Projectile new_projectile = Instantiate(projectile_prefab, projectile_spawn_point.transform.position, Quaternion.identity);
        new_projectile.transform.rotation = Quaternion.Euler(0, 0, input < 0 ? 0 : 180);

        new_projectile.attached_attack.SetOnHit(OnHit);
    }

    void OnHit(IDamageable hit, Attack hit_by) {
        character.DealDamage(character.power, hit);
    }
}
