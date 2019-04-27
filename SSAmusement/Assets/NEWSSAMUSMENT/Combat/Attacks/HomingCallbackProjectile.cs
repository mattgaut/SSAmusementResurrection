using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCallbackProjectile : HomingProjectile {

    [SerializeField] float max_distance_from_target;
    [SerializeField] bool only_hit_target;
    Action<Character> callback;

    [SerializeField] SingleHitAttack attack;

    public void SetTarget(Transform target, Action<Character> action) {
        SetTarget(target);
        SetCallback(action);
    }

    public void SetCallback(Action<Character> callback) {
        this.callback = callback;
        if (!only_hit_target) attack.SetOnHit((a, b) => { if (callback != null && a.character != null) HitNonTarget(a.character); });
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (target == null && ((1 << collision.gameObject.layer &  1 << LayerMask.NameToLayer("Wall")) != 0)) {
            Explode();
        } else if (target != null && collision.gameObject.transform.root == target.transform.root) {
            if (callback != null) callback.Invoke(target.gameObject.GetComponentInParent<Character>());
            Explode();
        } else {
            base.OnTriggerEnter2D(collision);
        }
    }

    void HitNonTarget(Character c) {
        callback.Invoke(c);
        Explode();
    }
}
