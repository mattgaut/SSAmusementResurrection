using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    public static T instance { get; private set; }

    void Awake() {
        if (instance == null) {
            instance = GetComponent<T>();
            DontDestroyOnLoad(instance);
            OnAwake();
        } else {
            Destroy(gameObject);
        }
    }

    protected virtual void OnAwake() { }

    //public static implicit operator T (Singleton<T> singleton) {
    //    return instance;
    //}
}
