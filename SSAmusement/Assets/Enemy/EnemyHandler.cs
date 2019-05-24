using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Enemy))]
public abstract class EnemyHandler : StateMachineController {

    protected CharacterController cont;
    [SerializeField] bool active_on_start;
    [SerializeField] Transform line_of_sight_origin;
    [SerializeField] protected LayerMask line_of_sight_blocking_mask;

    [SerializeField] bool need_line_of_sight;

    [SerializeField] float _aggro_range;

    [SerializeField] protected bool bump_damage;
    [SerializeField] protected Vector3 bump_knockback;

    float bump_cooldown = 1f;
    float last_bump;

    float acceleration_grounded = 0f;
    float acceleration_airborne = 0f;

    float x_smooth;

    protected bool knocked_back_last_frame;

    protected Coroutine ai_routine;

    protected float aggro_range { get { return _aggro_range; } }
    protected CharacterController.CollisionInfo collision_info { get { return cont.collisions; } }

    protected Enemy enemy { get; private set; }

    protected Player target;

    protected Vector2 _input;

    public Vector2 input { get { return _input; } set { _input = value; } }
    public override bool can_transition { get { return !GameManager.instance.IsTimeFrozen(enemy.team); } }

    public bool CanHunt() {
        return target != null && CustomCanHunt() && Vector2.Distance(target.transform.position, transform.position) <= aggro_range && (!need_line_of_sight || HasLineOfSight());
    }

    public virtual bool HasLineOfSight() {
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

    protected virtual bool CustomCanHunt() {
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (bump_damage && collision.gameObject.layer == LayerMask.NameToLayer("Player") && last_bump > bump_cooldown) {
            ConfirmBump(collision.gameObject.GetComponentInParent<Player>());
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (bump_damage && collision.gameObject.layer == LayerMask.NameToLayer("Player") && last_bump > bump_cooldown) {
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
            enemy.GiveKnockback(player, real_knockback);
        }
        last_bump = 0;
    }

    protected override void Awake() {
        base.Awake();
        cont = GetComponent<CharacterController>();
        enemy = GetComponent<Enemy>();
    }

    protected virtual void Start() {
        last_bump = bump_cooldown;
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
        enemy.animator.Rebind();
        _input = Vector2.zero;
        enemy.health.current = enemy.health;
    }

    protected virtual void Update() {
        last_bump += GameManager.GetDeltaTime(enemy.team);
    }
}
