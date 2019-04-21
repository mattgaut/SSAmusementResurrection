using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefCartHandler : EnemyHandler {

    [SerializeField] Transform raycast_origin;
    [SerializeField] LayerMask player_mask;
    [SerializeField] float max_charge_length;
    [SerializeField] float time_to_rest;
    Vector3 charge_knockback_bonus;
    float percent_bonus_speed;
    float acceleration_time = 0.3f;
    float smooth;

    protected override void Ini() {
        base.Ini();
        charge_knockback_bonus = new Vector3(10, 0, 0);
    }

    protected override bool CustomCanHunt() {
        RaycastHit2D hit = Physics2D.Raycast(raycast_origin.position, Vector2.left, aggro_range, player_mask);
        if (!hit) {
            hit = Physics2D.Raycast(raycast_origin.position, Vector2.right, aggro_range, player_mask);
        }
        if (!hit) {
            return false;
        } else {
            hit = Physics2D.Raycast(raycast_origin.position, hit.point - (Vector2)raycast_origin.position, hit.distance, line_of_sight_blocking_mask);
            return !hit;
        }
    }
    protected override float BumpDamage() {
        return enemy.power * (1 + percent_bonus_speed);
    }
    protected override void Bump(Player player) {
        enemy.DealDamage(BumpDamage(), player);
        Vector3 real_knockback = bump_knockback;
        real_knockback += charge_knockback_bonus * percent_bonus_speed;
        if (player.transform.position.x < transform.position.x) {
            real_knockback.x *= -1;
        }
        player.TakeKnockback(enemy, real_knockback);
    }
    protected override void Deactivate() {
        base.Deactivate();
        enemy.animator.SetBool("Charging", false);
    }

    IEnumerator Wander() {
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
            input.x = direction * enemy.speed;
            if (CanHunt()) {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Charge() {
        enemy.animator.SetBool("Charging", true);
        bump_damage = true;
        transform.position += Vector3.up * 0.01f;
        float direction = Mathf.Sign(target.transform.position.x - transform.position.x);

        float time = 0;
        float speed = enemy.speed;
        float accel_factor = 1.5f;
        while (!ShouldStopMoving(direction) && time < max_charge_length) {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
            if (speed < enemy.speed * 4f) {

                speed = enemy.speed * Mathf.Pow(time * Mathf.Pow(3, 1/accel_factor)/ 1.2f, accel_factor) + enemy.speed;
                if (speed >= enemy.speed * 4f) {
                    speed = enemy.speed * 4f;
                }
            }
            input.x = speed * direction;
            percent_bonus_speed = ((speed - enemy.speed) / (enemy.speed * 3f));

        }
        percent_bonus_speed = 0;
        input = Vector2.zero;
        enemy.animator.SetBool("Charging", false);
        bump_damage = false;
        float rest_time = time_to_rest;
        while (rest_time > 0) {
            rest_time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        enemy.animator.SetTrigger("EndRest");
    }

    protected override bool ShouldStopMoving(float direction) {
        if (direction < 0) {
            return collision_info.left || collision_info.hanging_left || collision_info.climbing_slope || base.ShouldStopMoving(direction);
        } else if (direction > 0) {
            return collision_info.right || collision_info.hanging_right || collision_info.climbing_slope || base.ShouldStopMoving(direction);
        }
        return base.ShouldStopMoving(direction);
    }
}
