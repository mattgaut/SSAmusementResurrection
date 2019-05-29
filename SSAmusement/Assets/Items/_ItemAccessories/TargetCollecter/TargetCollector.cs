using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollector : MonoBehaviour {

    List<Character> possible_targets;

    public Character GetRandomTarget() {
        if (possible_targets.Count > 0)
            return possible_targets.GetRandom();
        else
            return null;
    }

    private void Awake() {
        possible_targets = new List<Character>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & 1 << LayerMask.NameToLayer("Enemy")) != 0) {
            possible_targets.Add(collision.gameObject.GetComponentInParent<Character>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & LayerMask.GetMask("Enemy")) != 0) {
            possible_targets.Remove(collision.gameObject.GetComponentInParent<Character>());
        }
    }
}
