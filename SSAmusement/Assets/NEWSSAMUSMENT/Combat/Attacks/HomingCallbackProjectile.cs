using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCallbackProjectile : HomingProjectile {

    [SerializeField] float max_distance_from_target;
    Action callback;

    public void SetTarget(Transform target, Action action) {
        SetTarget(target);
        SetCallback(action);
    }

    public void SetCallback(Action callback) {
        this.callback = callback;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (target == null && ((1 << collision.gameObject.layer &  1 << LayerMask.NameToLayer("Wall")) != 0)) {
            Explode();
        } else if (target != null && collision.gameObject.transform.root == target.transform.root) {
            if (callback != null) callback.Invoke();
            Explode();
        } else {
            base.OnTriggerEnter2D(collision);
        }
    }
}
