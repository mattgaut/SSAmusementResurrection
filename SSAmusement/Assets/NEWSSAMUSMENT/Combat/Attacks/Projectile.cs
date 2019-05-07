using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    public SingleHitAttack attached_attack {
        get { return attack; }
    }

    [SerializeField] protected LayerMask break_mask, always_break_mask;
    [SerializeField] protected float speed, ignore_wall_timer, max_lifetime;

    [SerializeField] protected Vector3 base_direction;

    [SerializeField] ParticleSystem particles;

    [SerializeField] protected SingleHitAttack attack;

    Animator anim;
    Rigidbody2D rb;
    float timer = 0;
    bool is_exploded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        is_exploded = false;
    }

    private void Update() {
        if (is_exploded) {
            return;
        }

        ignore_wall_timer -= Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > max_lifetime && max_lifetime != 0) {
            Explode();
        }

        Turn();

        transform.position += transform.localRotation * base_direction * speed * Time.deltaTime;
    }

    protected virtual void Turn() {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (ignore_wall_timer <= 0 && ((1 << collision.gameObject.layer & break_mask) != 0)) {
            Explode();
        } else if ((1 << collision.gameObject.layer & always_break_mask) != 0) {
            Explode();
        }
    }

    public virtual void Explode() {
        speed = 0;
        if (particles) {
            particles.Stop();
            particles.gameObject.transform.SetParent(null);
        }
        anim.SetTrigger("Explode");
        is_exploded = true;
    }

    public void EndLife() {
        Destroy(gameObject);
    }
}
