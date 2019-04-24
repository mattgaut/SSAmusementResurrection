using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile {

    [SerializeField] protected Transform target;
    [SerializeField] float turn_speed;

    public void SetTarget(Transform trans) {
        target = trans;
    }

    protected override void Turn() {
        if (target == null) return;

        Vector3 current_trajectory = transform.rotation * base_direction;
        float angle = Vector2.SignedAngle(current_trajectory, target.transform.position - transform.position);
        if (turn_speed * Time.deltaTime > Mathf.Abs(angle)) {
            transform.rotation *= Quaternion.Euler(0, 0, (angle));
        } else {
            transform.rotation *= Quaternion.Euler(0, 0, (Mathf.Sign(angle) * turn_speed * Time.deltaTime));
        }
    }

    public override void Explode() {
        base.Explode();
        turn_speed = 0;
    }
}
