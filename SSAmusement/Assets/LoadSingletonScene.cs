using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSingletonScene : MonoBehaviour {
    void Awake() {
        SceneManager.LoadScene("Singletons", LoadSceneMode.Additive);    
    }
}
