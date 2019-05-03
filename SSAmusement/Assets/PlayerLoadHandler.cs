using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoadHandler : MonoBehaviour {

    private void Start() {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene loaded, LoadSceneMode mode) {
        foreach (Item i in GetComponent<Player>().inventory.items) {
            UIHandler.DisplayItem(i, false);
        }
    }
}
