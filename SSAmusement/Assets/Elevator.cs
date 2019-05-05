using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Elevator : MonoBehaviour, IInteractable {

    bool loading = false;

    public void Interact(Player player) {
        if (!loading) {
            StartCoroutine(FadeToBlackLoadLevel());
            loading = true;
        }
    }

    public void SetHighlight(bool is_highlighted) {
        
    }

    IEnumerator FadeToBlackLoadLevel() {
        yield return UIHandler.FadeToBlack(1f);
        GameManager.instance.LoadNextLevelAtRandom();
    }
}
