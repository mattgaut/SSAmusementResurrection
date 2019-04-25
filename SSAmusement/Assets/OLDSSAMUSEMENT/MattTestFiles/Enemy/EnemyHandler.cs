using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Enemy))]
public abstract class EnemyHandler : StateMachineController, IInputHandler {

    protected CharacterController cont;
    [SerializeField] bool active_on_start;
    [SerializeField] GameObject flip_object;
    [SerializeField] Transform line_of_sight_origin;
    [SerializeField] protected LayerMask line_of_sight_blocking_mask;

    [SerializeField] [Range(0, 20)] float jump_height = 4;
    [SerializeField] [Range(0, 5)] float time_to_jump_apex = .4f;
    [SerializeField] bool no_gravity, need_line_of_sight;

    [SerializeField] float _aggro_range;

    [SerializeField] protected bool bump_damage;
    [SerializeField] protected Vector3 bump_knockback;
    float bump_cooldown = 1f;
    float last_bump;

    float acceleration_grounded = 0f;
    float acceleration_airborne = 0f;

    float gravity;
    float jump_velocity;
    float x_smooth;

    bool knocked_back_last_frame;

    Vector3 velocity, gravity_force;

    protected Coroutine ai_routine;

    Coroutine drop_routine;

    protected bool can_flip = true;

    protected float aggro_range { get { return _aggro_range; } }
    protected CharacterController.CollisionInfo collision_info { get { return cont.collisions; } }

    protected Enemy enemy { get; private set; }

    protected Player target;

    protected Vector2 _input;

    public Vector2 input { get { return _input; } set { _input = value; } }

    public int facing { get; private set; }

    public bool CanHunt() {
        return target != null && CustomCanHunt() && Vector2.Distance(target.transform.position, transform.position) <= aggro_range && (!need_line_of_sight || HasLineOfSight());
    }
    protected virtual bool CustomCanHunt() {
        return true;
    }

    protected virtual bool HasLineOfSight() {
        CharacterDefinition target_definition = target.char_definition;
        RaycastHit2D hit = Physics2D.Linecast(line_of_sight_origin.position, target_definition.center_mass.position, line_of_sight_blocking_mask);
        if (hit) {
            hit = Physics2D.Linecast(line_of_sight_origin.position, target_definition.head.position, line_of_sight_blocking_mask);
            if (hit) {
                hit = Physics2D.Linecast(line_of_sight_origin.position, target_definition.feet.position, line_of_sight_blocking_mask);
            }
        }
        return !hit;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (bump_damage && collision.gameObject.layer == LayerMask.NameToLayer("Player") && last_bump > bump_cooldown) {
            last_bump = Time.deltaTime;
            ConfirmBump(collision.gameObject.GetComponentInParent<Player>());
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (bump_damage && collision.gameObject.layer == LayerMask.NameToLayer("Player") && last_bump > bump_cooldown) {
            last_bump = Time.deltaTime;
            ConfirmBump(collision.gameObject.GetComponentInParent<Player>());
        }
    }

    protected void ConfirmBump(Player player) {
        if (player.invincible) {
            last_bump = 0;
        } else {
            Bump(player);
        }
    }

    protected virtual void Bump(Player player) {
        enemy.DealDamage(BumpDamage(), player);
        if (bump_knockback != Vector3.zero) {
            Vector3 real_knockback = bump_knockback;
            if (player.transform.position.x < transform.position.x) {
                real_knockback.x *= -1;
            }
            player.TakeKnockback(enemy, real_knockback);
        }
        last_bump = 0;
    }

    protected override void Awake() {
        base.Awake();
        cont = GetComponent<CharacterController>();
        enemy = GetComponent<Enemy>();

        gravity = -(2 * jump_height) / (time_to_jump_apex * time_to_jump_apex);
        if (no_gravity) gravity = 0;
        jump_velocity = time_to_jump_apex * Mathf.Abs(gravity);
    }

    protected void Start() {
        Ini();
        if (active_on_start) {
            target = FindObjectOfType<Player>();
        }
        if (!target) {
            target = FindObjectOfType<Player>();
        }
        SetActive(active_on_start);
    }

    protected virtual void Ini() {}

    protected virtual float BumpDamage() {
        return enemy.power / 2;
    }

    protected override void Deactivate() {
        base.Deactivate();
        _input = Vector2.zero;
        enemy.animator.Rebind();
        enemy.health.current = enemy.health;
        can_flip = true;
    }

    protected virtual void Update() {
        last_bump += Time.deltaTime;
        Move();
    }

    void Move() {
        Vector3 movement = Vector3.zero;
        if (cont.collisions.above || cont.collisions.below) {
            velocity.y = 0;
            gravity_force.y = 0;
        }
        gravity_force.y += gravity * Time.fixedDeltaTime;

        if (!enemy.is_knocked_back) {
            knocked_back_last_frame = false;
            if (_input.y > 0 && cont.collisions.below) {
                velocity.y = jump_velocity;
            }
            if (_input.y < 0 && drop_routine == null) {
                drop_routine = StartCoroutine(DropRoutine());
            }

            velocity.x = _input.x;
            movement = (velocity + gravity_force) * Time.deltaTime;
            Face(movement.x);
        } else {
            if (knocked_back_last_frame == false) gravity_force = Vector3.zero;
            knocked_back_last_frame = true;
            velocity.y = 0;
            movement = ((gravity_force + enemy.knockback_force) * Time.deltaTime);
        }

        cont.Move(movement);

        if (enemy.is_knocked_back && (cont.collisions.left || cont.collisions.right)) {
            enemy.CancelXKnockBack();
        }
        if (enemy.is_knocked_back && cont.collisions.above) {
            enemy.CancelYKnockBack();
        }
        if (enemy.is_knocked_back && cont.collisions.below) {
            enemy.CancelKnockBack();
        }
    }

    IEnumerator DropRoutine() {
        cont.RemovePlatformFromMask();
        float delay = .15f;
        while (delay > 0) {
            yield return new WaitForFixedUpdate();
            delay -= Time.fixedDeltaTime;
        }
        while (Input.GetKey(KeyCode.S)) {
            yield return new WaitForFixedUpdate();
        }
        cont.AddPlatformToMask();
        drop_routine = null;
    }

    public void Face(float i) {
        if (!can_flip) return;
        if (i > 0) {
            flip_object.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            facing = 1;
        } else if (i < 0) {
            flip_object.transform.localRotation = Quaternion.Euler(0, 0, 0);
            facing = -1;
        }
    }
    protected bool ShouldStopMoving(int direction) {
        return ShouldStopMoving((float)direction);
    }
    protected virtual bool ShouldStopMoving(float direction) {
        return collision_info.past_max;
    }
}
