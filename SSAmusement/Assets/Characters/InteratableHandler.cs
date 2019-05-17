using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteratableHandler : MonoBehaviour {

    Player player;

    IInteractable over;

    float last_input, input;

    private void Awake() {
        player = GetComponentInParent<Player>();
    }

    private void Update() {
        input = Input.GetAxisRaw("Interact");
        if (input != 0 && last_input != input && over != null) {
            over.Interact(player);
        }
        last_input = input;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (new_interacable != null) {
            if (over != null) over.SetHighlight(false);
            over = new_interacable;
            over.SetHighlight(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        IInteractable new_interacable = collision.gameObject.GetComponent<IInteractable>();
        if (over == null && new_interacable != null) {
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
