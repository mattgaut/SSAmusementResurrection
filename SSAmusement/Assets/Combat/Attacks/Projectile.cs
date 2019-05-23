using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : SingleHitAttack {

    [SerializeField] protected LayerMask break_mask, break_after_timer_mask;
    [SerializeField] protected float speed, ignore_wall_timer, max_lifetime;

    [SerializeField] protected float gravity_force;

    [SerializeField] protected Vector3 base_direction;

    [SerializeField] ParticleSystem particles;

    [SerializeField] SFXInfo explode_sfx;

    Animator anim;
    Rigidbody2D rb;
    bool is_exploded;

    Vector3 gravity_vector;

    public void SetSpeedAndDirection(Vector2 force) {
        speed = force.magnitude;
        base_direction = force.normalized;
    }

    public override void SetSource(Character source) {
        base.SetSource(source);

        GameManager.instance.AddOnTimeScaleChangedEvent(source.team, OnChangeTimeScale);
    }

    public void Flip() {
        base_direction.x = -base_direction.x;
    }

    public void EndLife() {
        if (source != null) GameManager.instance.RemoveOnTimeScaleChangedEvent(source.team, OnChangeTimeScale);
        Destroy(gameObject);
    }

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

        float time_step = GameManager.GetDeltaTime(source?.team);

        if (ignore_wall_timer >= 0) {
            ignore_wall_timer -= time_step;
            if (ignore_wall_timer < 0) {
                break_mask = break_after_timer_mask | break_mask;
            }
        }
        if (timer > max_lifetime && max_lifetime != 0) {
            Explode();
        }

        Turn();

        transform.position += transform.localRotation * base_direction * speed * time_step;
        transform.position += gravity_vector * time_step;

        gravity_vector += Vector3.down * gravity_force * time_step;
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

    protected virtual void Explode() {
        speed = 0;
        SoundManager.instance?.LocalPlaySfx(explode_sfx);
        if (particles) {
            particles.Stop();
            particles.gameObject.transform.SetParent(null);
        }
        if (anim != null && anim.enabled) {
            anim.SetTrigger("Explode");
        } else {
            EndLife();
        }
        is_exploded = true;

        hitbox.enabled = false;
    }

    protected virtual void OnChangeTimeScale(float time_scale) {
        if (anim != null) anim.speed = time_scale;
        if (particles != null) {
            ParticleSystem.MainModule main = particles.main;
            main.simulationSpeed = time_scale;
        }
    }
}
