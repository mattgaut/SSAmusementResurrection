using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSingletonScene : MonoBehaviour {
    void Start() {
        SceneManager.LoadScene("Singletons", LoadSceneMode.Additive);    
    }
}
