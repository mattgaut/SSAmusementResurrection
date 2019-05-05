using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemyHandler : EnemyHandler, IInputHandler {

    [SerializeField] GameObject flip_object;

    /*[SerializeField] [Range(0, 20)] */float jump_height = 1.6f;
    /*[SerializeField] [Range(0, 5)] */float time_to_jump_apex = 0.24f;
    [SerializeField] bool no_gravity;

    protected bool can_flip = true;

    public int facing { get; private set; }

    float gravity;
    float jump_velocity;

    Vector2 gravity_force;
    Vector2 velocity;

    Coroutine drop_routine;

    protected override void Awake() {
        base.Awake();

        gravity = -(2 * jump_height) / (time_to_jump_apex * time_to_jump_apex);
        if (no_gravity) gravity = 0;
        jump_velocity = time_to_jump_apex * Mathf.Abs(gravity);
    }

    protected override void Update() {
        base.Update();
        Move();
    }

    protected override void Deactivate() {
        base.Deactivate();
        can_flip = true;
    }

    void Move() {
        Vector2 movement = Vector2.zero;
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
            if (knocked_back_last_frame == false) gravity_force = Vector2.zero;
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
