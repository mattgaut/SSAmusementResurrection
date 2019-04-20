using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Ability : MonoBehaviour {

    public Character character { get; private set; }

    [SerializeField] protected string _ability_name;
    [SerializeField] protected Sprite _icon;

    protected virtual void Awake() {
        character = GetComponent<Character>();
    }
}
