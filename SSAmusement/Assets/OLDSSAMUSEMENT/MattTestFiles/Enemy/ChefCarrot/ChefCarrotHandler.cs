using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefCarrotHandler : EnemyHandler {

    [SerializeField] GameObject carrot;
    [SerializeField] Transform carrot_spawn_transform;

    float time_between_throws = 1.8f;
    float last_throw;

    bool throw_released;

    public bool is_attack_ready {
        get { return last_throw > time_between_throws; }
    }
    public bool is_too_close {
        get { return Vector2.Distance(target.transform.position, transform.position) < 3; }
    }
    public bool is_too_far {
        get { return Vector2.Distance(target.transform.position, transform.position) > 6; }
    }

    protected override void Update() {
        base.Update();
        last_throw += Time.deltaTime;
    }

    protected override void Activate() {
        base.Activate();
        last_throw = 0;
    }
    protected override void Deactivate() {
        base.Deactivate();
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
            input.x = direction * enemy.speed;
            if (CanHunt()) {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        input.x = 0;
    }

    protected IEnumerator Hunt() {
        enemy.animator.SetBool("Mad", true);
        Face(target.transform.position.x - transform.position.x);
        yield return new WaitForFixedUpdate();
    }

    protected IEnumerator ThrowCarrot() {
        throw_released = false;
        Face(target.transform.position.x - transform.position.x);
        last_throw = 0;
        enemy.animator.SetTrigger("Attack");

        while (!throw_released) {
            Face(target.transform.position.x - transform.position.x);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
    }

    protected IEnumerator WalkTowardsTarget() {
        float direction = target.transform.position.x - transform.position.x;
        if (!ShouldStopMoving(direction)) {
            input.x = Mathf.Sign(direction) * enemy.speed;
        }
        yield return new WaitForFixedUpdate();
        input.x = 0;
    }
    protected IEnumerator WalkAwayFromTarget() {
        float direction = -target.transform.position.x + transform.position.x;
        if (!ShouldStopMoving(direction)) {
            input.x = Mathf.Sign(direction) * enemy.speed;
        }
        yield return new WaitForFixedUpdate();
        input.x = 0;
    }

    protected override bool ShouldStopMoving(float direction) {
        if (direction < 0) {
            return collision_info.left || collision_info.hanging_left || base.ShouldStopMoving(direction);
        } else if (direction > 0) {
            return collision_info.right || collision_info.hanging_right || base.ShouldStopMoving(direction);
        }
        return base.ShouldStopMoving(direction);
    }

    public void AnimReleaseThrow() {
        throw_released = true;

        GameObject new_carrot = Instantiate(carrot);
        new_carrot.transform.position = carrot_spawn_transform.position;
        float angle = 0;
        Vector3 to_target = target.transform.position - transform.position;
        if (to_target.x < 0) {
            angle = Vector2.SignedAngle(Vector2.left, to_target);
            angle = Mathf.Clamp(angle, -45, 45);
        } else {
            angle = Vector2.SignedAngle(Vector2.right, to_target) + 180;
            angle = Mathf.Clamp(angle, 135, 225);
        }
        new_carrot.transform.rotation = Quaternion.Euler(0, 0, angle);

        new_carrot.GetComponent<Attack>().SetOnHit((hit, attack) => enemy.DealDamage(enemy.power, hit));
    }
}
