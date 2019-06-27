using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityTargetCollector : TargetCollector {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Enemy")) != 0) {
            Character character = collision.gameObject.GetComponentInParent<Character>();
            targets.Add(character);
            character.on_death += OnDeathRemoveTarget;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Enemy")) != 0) {
            Character character = collision.gameObject.GetComponentInParent<Character>();
            targets.Remove(character);
            character.on_death -= OnDeathRemoveTarget;
        }
    }

    void OnDeathRemoveTarget(Character killed, Character killed_by) {
        RemoveTarget(killed);
    }
}
