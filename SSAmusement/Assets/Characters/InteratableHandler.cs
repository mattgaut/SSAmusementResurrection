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
        if (over != null && MyInput.GetButtonDown("Interact")) {
            over.Interact(player);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (new_interacable != null && new_interacable.is_available) {
            if (new_interacable.is_available) {
                if (over != null) over.SetHighlight(false);
                over = new_interacable;
                over.SetHighlight(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (over == null && new_interacable != null && new_interacable.is_available) {
            over = new_interacable;
            over.SetHighlight(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (over == new_interacable) {
            if (over != null) over.SetHighlight(false);
            over = null;
        }
    }
}
