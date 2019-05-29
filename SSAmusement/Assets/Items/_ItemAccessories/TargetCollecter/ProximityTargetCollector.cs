using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTargetCollector : TargetCollector {

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & 1 << LayerMask.NameToLayer("Enemy")) != 0) {
            targets.Add(collision.gameObject.GetComponentInParent<Character>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & LayerMask.GetMask("Enemy")) != 0) {
            targets.Remove(collision.gameObject.GetComponentInParent<Character>());
        }
    }
}
