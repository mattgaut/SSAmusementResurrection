using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCallbackProjectile : HomingProjectile {

    [SerializeField] float max_distance_from_target;
    [SerializeField] bool only_hit_target;
    Action<Character> callback;

    public void SetTarget(Transform target, Action<Character> action) {
        SetTarget(target);
        SetCallback(action);
    }

    public void SetCallback(Action<Character> callback) {
        this.callback = callback;
        //SetOnHit();
    }

    protected override void CheckHitboxCollisions(Collider2D collision) {
        if (target == null) {
            //if () {

            //}
            if ((1 << collision.gameObject.layer & break_mask) != 0) {
                Explode();
            }
        } else if (collision.gameObject.transform.root == target.transform.root) {
            if (callback != null) callback.Invoke(target.gameObject.GetComponentInParent<Character>());
            Explode();
        }
    }

    void HitNonTarget(Character c) {
        callback.Invoke(c);
        Explode();
    }
}
