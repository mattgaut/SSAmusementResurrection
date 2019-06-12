using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightEnemyHandler : EnemyHandler {

    bool finished = false;
    protected IEnumerator AIRoutine() {
        while (true) {
            yield return null;
            _input.x = 1;

            if (cont.collisions.right && !finished) {
                Debug.Log("Done: " + Time.time);
                finished = true;
            }
        }        
    }
}
