using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CounterHitbox : MonoBehaviour {

    [SerializeField] LayerMask targets;
    Collider2D coll;

    public bool was_hit {
        get; private set;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & targets) != 0) {
            was_hit = true;
        }        
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & targets) != 0) {
            was_hit = true;
        }
    }

    private void Awake() {
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        was_hit = false;
        coll.enabled = true;
    }

    private void OnDisable() {
        coll.enabled = false;
        was_hit = false;
    }

}
