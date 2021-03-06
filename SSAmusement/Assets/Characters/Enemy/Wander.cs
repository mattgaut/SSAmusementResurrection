﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : GroundedEnemyHandler {

    [SerializeField] bool get_mad;

    protected override void Ini() {
        base.Ini();
        SetActive(true);
    }

    protected override bool CustomCanHunt() {
        return false;
    }

    protected IEnumerator AIRoutine() {
        if (get_mad) enemy.animator.SetTrigger("Mad");
        while (true) {
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
                wander_length -= GameManager.GetFixedDeltaTime(enemy.team);
                _input.x = direction;
                yield return new WaitForFixedUpdate();
            }
        }

    }
}
