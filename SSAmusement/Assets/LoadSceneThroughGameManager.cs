using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneThroughGameManager : MonoBehaviour {

    [SerializeField] string scene;
    [SerializeField] LoadSceneMode mode;

    [SerializeField] Player selection;

    public void Load() {
        GameManager.instance.LoadScene(scene, mode);
    }

    public void StartGame() {
        GameManager.instance.StartGame(selection);
    }
}
