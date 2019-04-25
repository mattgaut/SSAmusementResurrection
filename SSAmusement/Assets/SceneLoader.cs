using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    [SerializeField] string to_load;

    private void Start() {
        if (to_load != "") {
            LoadScene(to_load);
        }
    }

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }
    public void LoadScene(int scene) {
        SceneManager.LoadScene(scene);
    }
    public void Quit() {
        Application.Quit();
    }
}
