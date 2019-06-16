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

    public Sprite icon {
        get { return attached_item.icon; }
    }

    public bool is_unlocked {
        get; private set;
    }
    public int progress {
        get { return tracker.value; }
    }

    [SerializeField] string _achievement_name;
    [SerializeField][TextArea(1, 5)] string _description;

    [SerializeField] Item attached_item;

    [SerializeField] NumericStatistic tracker;
    [SerializeField] int target_value;

    [System.Serializable]
    public class Data {
        public string name;
        public bool is_unlocked;

        public Statistic.Data tracked_progress;
    }
}
