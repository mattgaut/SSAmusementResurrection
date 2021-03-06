﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile {

    [SerializeField] protected Transform target;
    [SerializeField] bool can_only_hit_target, can_break_when_chasing;
    [SerializeField] float turn_speed;

    Character target_character;

    public void SetTarget(Character character) {
        target_character = character;
        if (target_character) {
            target = character.stats.center_mass;
        } else {
            target = null;
        }
    }

    protected override void Turn() {
        if (target == null) return;

        Vector3 current_trajectory = transform.rotation * base_direction;
        float angle = Vector2.SignedAngle(current_trajectory, target.transform.position - transform.position);
        if (turn_speed * GameManager.GetDeltaTime(source?.team) > Mathf.Abs(angle)) {
            transform.rotation *= Quaternion.Euler(0, 0, (angle));
        } else {
            transform.rotation *= Quaternion.Euler(0, 0, (Mathf.Sign(angle) * turn_speed * GameManager.GetDeltaTime(source?.team)));
        }
    }

    protected override bool HitCondition(Character d) {
        return target == null || target_character == d;
    }

    protected override void CheckHitboxCollisions(Collider2D collision) {
        if (target == null) {
            if (!can_only_hit_target) {
                base.CheckHitboxCollisions(collision);
            } else {
                if ((1 << collision.gameObject.layer & break_mask) != 0) {
                    Explode();
                }
            }
        } else if (!can_break_when_chasing) {
            if ((1 << collision.gameObject.layer & targets) != 0) {
                ConfirmHit(collision.gameObject.GetComponentInParent<Character>());
            }
        } else {
            base.CheckHitboxCollisions(collision);
        }
    }

    protected override void Explode() {
        base.Explode();
        turn_speed = 0;
    }
}
