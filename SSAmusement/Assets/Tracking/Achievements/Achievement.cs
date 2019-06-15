using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement", order = 0)]
public class Achievement : ScriptableObject {

    public string achievement_name {
        get { return _achievement_name; }
    }

    public string description {
        get { return _description; }
    }

    [SerializeField] string _achievement_name;
    [SerializeField][TextArea(1, 5)] string _description;


}
