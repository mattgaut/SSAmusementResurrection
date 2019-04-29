using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialEnemyHandler : EnemyHandler {

    Vector2 velocity;
    Vector2 smooth_damp_velocity;
    float smooth_damp_time = 1f;
    [SerializeField] GameObject pivot_object;

    protected IEnumerator Wander() {
        Vector2 direction = Vector2.zero;
        if (Random.Range(0f, 1f) < 1f) {
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
        }        

        float wander_length = Random.Range(0.5f, 2f);
        while (wander_length > 0) {
            wander_length -= Time.fixedDeltaTime;
            _input = direction;
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

        if (!enemy.is_knocked_back) {
            velocity = Vector2.SmoothDamp(velocity, input, ref smooth_damp_velocity, 0.5f);
            Tilt(velocity.x);
            movement = velocity * enemy.speed * Time.deltaTime;
            //Face(movement.x);
        } else {
            Tilt(enemy.knockback_force.x / enemy.speed);
            movement = enemy.knockback_force * Time.deltaTime;
            velocity = movement;
        }

        cont.Move(movement);
    }

    void Tilt(float input) {
        pivot_object.transform.localRotation = Quaternion.Euler(0,0, -Mathf.Clamp(input, -1f, 1f) * 45f);
    }
}
