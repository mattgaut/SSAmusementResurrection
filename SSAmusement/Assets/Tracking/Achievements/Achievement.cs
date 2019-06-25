using System;
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
        get { return attached_item?.icon; }
    }

    public bool is_unlocked {
        get; private set;
    }
    public int progress {
        get { return tracker.value; }
    }
    public int target {
        get { return target_value; }
    }
    public NumericStatistic tracked_statistic {
        get { return tracker; }
    }

    public event Action<Achievement> on_unlock;

    [SerializeField] string _achievement_name;
    [SerializeField] [TextArea(1, 5)] string _description;

    [SerializeField] Item attached_item;

    [SerializeField] bool has_unique_tracker = false;
    [SerializeField] NumericStatistic tracker;
    [SerializeField] int target_value = 1;

    public Data Save() {
        return new Data(this);
    }
    public void Load(Data data) {
        if (data == null) {
            //is_unlocked = false;
        } else {
            is_unlocked = data.is_unlocked;

            if (!is_unlocked) {
                if (has_unique_tracker) {
                    tracker.Load(data.tracked_progress);
                }
            } else {
                tracker.on_value_changed -= CheckUnlocked;
            }
        }
    }

    public void Reset() {

        // Add callback back to tracker. Remove first in case achievement was already being tracked.
        if (tracker) {
            tracker.on_value_changed -= CheckUnlocked;
            tracker.on_value_changed += CheckUnlocked;
        }

        is_unlocked = false;
        if (has_unique_tracker) {
            tracker.Clear();
        }
    }

    private void OnEnable() {
        if (tracker) {
            tracker.on_value_changed += CheckUnlocked;
        }
    }

    private void OnDisable() {
        if (tracker) {
            tracker.on_value_changed -= CheckUnlocked;
        }
    }

    void CheckUnlocked(int value) {
        if (value >= target_value) {
            is_unlocked = true;
            tracker.on_value_changed -= CheckUnlocked;
            on_unlock?.Invoke(this);
        }
    }

    [System.Serializable]
    public class Data {
        public string name;
        public bool is_unlocked;

        public Statistic.Data tracked_progress;

        public Data(Achievement achievement) {
            name = achievement.achievement_name;
            is_unlocked = achievement.is_unlocked;

            tracked_progress = achievement.tracker.Save();
        }
    }
}
