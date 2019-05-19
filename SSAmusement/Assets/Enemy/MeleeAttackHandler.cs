using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackHandler : GroundedEnemyHandler {

    [SerializeField] float time_between_attacks;
    [SerializeField] Attack attack;
    [SerializeField] float attack_range;
    [SerializeField] Vector3 attack_knockback;
    [SerializeField] float close_distance;

    bool attack_over;
    float last_attack;

    public bool IsInAttackRange() {
        return Vector2.Distance(target.transform.position, transform.position) < attack_range;
    }

    public bool AttackReady() {
        return time_between_attacks < last_attack;
    }

    protected override void Ini() {
        base.Ini();
        attack.SetOnHit((hit, attack) => AttackOnHit(hit));
    }

    protected override void Update() {
        base.Update();
        last_attack += Time.deltaTime;
    }

    protected override void Deactivate() {
        base.Deactivate();
        attack_over = true;
        enemy.animator.SetBool("Mad", false);
    }

    protected IEnumerator Wander() {
        enemy.animator.SetBool("Mad", false);
        float direction;
        if (collision_info.left) {
            direction = 1;
        } else if (collision_info.right) {
            direction = -1;
        } else {
            direction = Random.Range(-1, 2);
        }
        float wander_length = Random.Range(0.5f, 2f);
        while (!ShouldStopMoving(direction) && wander_length > 0) {
            wander_length -= Time.fixedDeltaTime;
            _input.x = direction;
            if (CanHunt()) {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        _input.x = 0;
    }

    protected IEnumerator Hunt() {
        enemy.animator.SetBool("Mad", true);
        Face(target.transform.position.x - transform.position.x);
        _input.x = 0;
        yield return new WaitForFixedUpdate();
    }

    protected IEnumerator Attack() {
        Face(target.transform.position.x - transform.position.x);
        can_flip = false;
        last_attack = 0;
        enemy.animator.SetTrigger("Attack");

        _input.x = 0;

        attack_over = false;
        while (!attack_over) {
            yield return new WaitForFixedUpdate();
        }
        yield return null;
        can_flip = true;
    }

    protected IEnumerator WalkTowardsTarget() {
        float direction = target.transform.position.x - transform.position.x;
        if (!ShouldStopMoving(direction) && Mathf.Abs(direction) > 0.05f) {
            _input.x = Mathf.Sign(direction);
        } else {
            _input.x = 0;
        }
        yield return new WaitForFixedUpdate();
    }

    protected override bool ShouldStopMoving(float direction) {
        if (direction < 0) {
            return collision_info.left || collision_info.hanging_left || base.ShouldStopMoving(direction);
        } else if (direction > 0) {
            return collision_info.right || collision_info.hanging_right || base.ShouldStopMoving(direction);
        }
        return base.ShouldStopMoving(direction);
    }

    protected virtual void AttackOnHit(Character c) {
        enemy.DealDamage(enemy.power, c);
        if (attack_knockback != Vector3.zero) {
            Vector3 real_knockback = attack_knockback;
            if (c.gameObject.transform.position.x < transform.position.x) {
                real_knockback.x *= -1;
            }
            enemy.GiveKnockback(c, real_knockback);
        }
    }

    public void AnimDisableHitbox() {
        attack.Disable();
    }
    public void AnimAttackOver() {
        attack_over = true;
    }
    public void AnimEnableHitbox() {
        attack.Enable();
    }
}
