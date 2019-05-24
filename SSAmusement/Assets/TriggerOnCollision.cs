using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnCollision : MonoBehaviour {
    [SerializeField] LayerMask triggers_on;
    [SerializeField] UnityEvent on_trigger;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & triggers_on) != 0) {
            on_trigger.Invoke();
        }
    }
}
