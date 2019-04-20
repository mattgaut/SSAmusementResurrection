using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteratableHandler : MonoBehaviour {

    Player player;

    IInteractable over;

    private void Awake() {
        player = GetComponentInParent<Player>();
    }

    private void Update() {
        if (Input.GetButtonDown("Interact") && over != null) {
            over.Interact(player);
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (new_interacable != null) {
            over = new_interacable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (over == new_interacable) {
            over = null;
        }
    }
}
