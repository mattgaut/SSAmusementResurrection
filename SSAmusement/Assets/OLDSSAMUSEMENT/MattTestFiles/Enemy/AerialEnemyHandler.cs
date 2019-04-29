using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialEnemyHandler : EnemyHandler {

    Vector3 velocity;

    protected IEnumerator Wander() {
        Vector2 direction;
        float max_angle = 360, min_angle = 0;
        direction = Vector2.up;
        if (collision_info.left) {
            if (collision_info.above) {
                max_angle = 180;
                min_angle = 90;
            } else if (collision_info.below) {
                max_angle = 90;
            } else {
                max_angle = 180;
            }
        } else if (collision_info.right) {
            if (collision_info.above) {
                max_angle = 270;
                min_angle = 180;
            } else if (collision_info.below) {
                min_angle = 270;
            } else {
                min_angle = 180;
            }
        } else {
            if (collision_info.above) {
                max_angle = 270;
                min_angle = 90;
            } else if (collision_info.below) {
                min_angle = -90;
                max_angle = 90;
            }
        }

        direction = Quaternion.Euler(0, 0, -Random.Range(min_angle, max_angle)) * direction;

        float wander_length = Random.Range(0.5f, 2f);
        while (wander_length > 0) {
            wander_length -= Time.fixedDeltaTime;
            _input = direction * enemy.speed;
            if (CanHunt()) {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        _input = Vector2.zero;
    }

    protected override void Update() {
        base.Update();
        Move();
    }

    void Move() {
        Vector3 movement = Vector3.zero;
        if (cont.collisions.above || cont.collisions.below) {
            velocity.y = 0;
        }

        if (!enemy.is_knocked_back) {
            velocity = _input;
            movement = velocity * Time.deltaTime;
            //Face(movement.x);
        } else {
            velocity.y = 0;
            movement = enemy.knockback_force * Time.deltaTime;
        }

        cont.Move(movement);
    }
}
