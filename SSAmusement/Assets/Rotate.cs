using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    [SerializeField] Character.Team team;
    [SerializeField] float speed;

	// Update is called once per frame
	void Update () {
        transform.localRotation *= Quaternion.Euler(0, 0, speed * GameManager.GetDeltaTime(team));
	}
}
