﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : SingleHitAttack {

    [SerializeField] protected LayerMask break_mask, break_after_timer_mask;
    [SerializeField] protected float speed, ignore_wall_timer, max_lifetime;

    [SerializeField] protected Vector3 base_direction;

    [SerializeField] ParticleSystem particles;

    Animator anim;
    Rigidbody2D rb;
    bool is_exploded;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        is_exploded = false;
    }

    protected override void Update() {
        if (is_exploded) {
            return;
        }
        base.Update();

        if (ignore_wall_timer >= 0) {
            ignore_wall_timer -= Time.deltaTime;
            if (ignore_wall_timer < 0) {
                break_mask = break_after_timer_mask | break_mask;
            }
        }
        if (timer > max_lifetime && max_lifetime != 0) {
            Explode();
        }

        Turn();

        transform.position += transform.localRotation * base_direction * speed * Time.deltaTime;
    }

    protected virtual void Turn() {

    }

    protected override void CheckHitboxCollisions(Collider2D collision) {
        base.CheckHitboxCollisions(collision);
        if (((1 << collision.gameObject.layer & break_mask) != 0)) {
            Explode();
        }
    }

    protected override void OnCollisionWithTarget() {
        base.OnCollisionWithTarget();
        Explode();
    }

    public virtual void Explode() {
        speed = 0;
        if (particles) {
            particles.Stop();
            particles.gameObject.transform.SetParent(null);
        }
        anim.SetTrigger("Explode");
        is_exploded = true;

        hitbox.enabled = false;
    }

    public void EndLife() {
        Destroy(gameObject);
    }
}
