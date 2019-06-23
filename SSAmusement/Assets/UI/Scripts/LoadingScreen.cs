using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    [SerializeField] Animator anim;

    [SerializeField] Canvas canvas;

    public void StartLoadingScreen() {
        canvas.enabled = true;
    }

    public void EndLoadingScreen() {
        canvas.enabled = false;
    }
}
