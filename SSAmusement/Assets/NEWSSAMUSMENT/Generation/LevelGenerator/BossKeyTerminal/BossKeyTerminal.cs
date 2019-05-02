using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossKeyTerminal : MonoBehaviour, IInteractable {

    [SerializeField] Teleporter teleporter;
    [SerializeField] Image key_image;

    bool used;

    public void Interact(Player player) {
        if (!used) {
            if (player.inventory.TrySpendKeycard()) Use();
        }
    }

    public void Use() {
        teleporter.SetOpen(true);
        key_image.enabled = false;
    }

    public void SetHighlight(bool is_highlighted) {

    }
}
