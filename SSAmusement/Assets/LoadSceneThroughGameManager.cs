using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneThroughGameManager : MonoBehaviour {

    [SerializeField] string scene;
    [SerializeField] LoadSceneMode mode;

    public void Load() {
        GameManager.instance.LoadScene(scene, mode);
    }
}
