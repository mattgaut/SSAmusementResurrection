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

    public Data Save() {
        return new Data(this);
    }
    public void Load(Data data) {        
        is_unlocked = data.is_unlocked;

        if (!is_unlocked) {
            tracker.Load(data.tracked_progress);
        }
    }

    private void OnEnable() {
        tracker.on_value_changed += (value) => CheckUnlocked(value);
    }

    void CheckUnlocked(int value) {
        if (value >= target_value) {
            is_unlocked = true;
        }
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

        public Data(Achievement achievement) {
            name = achievement.name;
            is_unlocked = achievement.is_unlocked;

            tracked_progress = achievement.tracker.Save();
        }
    }
}
