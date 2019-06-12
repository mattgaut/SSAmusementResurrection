using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterID", menuName = "ScriptableObjects/CharacterID", order = 0)]
public class CharacterID : ScriptableObject {

    [SerializeField] string _character_name;
    [SerializeField] Character.Team _type;
    [SerializeField] GameObject _prefab;

    [Space(10)][TextArea(1, 6)][SerializeField] string _description;

    public string character_name {
        get { return _character_name; }
    }
    public Character.Team type {
        get { return _type; }
    }
    public GameObject prefab {
        get { return _prefab; }
    }

    public string description {
        get { return _description; }
    }
}
