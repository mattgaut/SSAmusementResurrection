using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightEnemyHandler : EnemyHandler {

    bool finished = false;
    // TODO Update TO State Machine
    protected IEnumerator AIRoutine() {
        while (true) {
            yield return null;
            input.x = enemy.speed;

            if (cont.collisions.right && !finished) {
                Debug.Log("Done: " + Time.time);
                finished = true;
            }
        }        
    }
}
