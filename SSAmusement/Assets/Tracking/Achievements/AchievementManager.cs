using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

public class AchievementManager : Singleton<AchievementManager> {

    static string save_location = "/Stats/Achievements.ssa";

    [SerializeField] List<Achievement> achievements;
    [SerializeField] StatisticTrackerManager tracker;

    Dictionary<string, Achievement> achievement_dict;

    [Space(10f)][SerializeField] UnityEventAchievement on_achievement_unlocked;

    protected override void OnAwake() {
        base.OnAwake();

        on_achievement_unlocked.AddListener((Achievement a) => UIHandler.DisplayAchievement(a));

        achievement_dict = new Dictionary<string, Achievement>();
        foreach (Achievement achievement in achievements) {
            achievement.Reset();

            achievement_dict.Add(achievement.achievement_name, achievement);
            achievement.on_unlock += on_achievement_unlocked.Invoke;
        }
    }

    protected void Start() {
        foreach (Achievement achievement in achievement_dict.Values) {
            if (!achievement.is_unlocked) tracker.AddStatistic(achievement.tracked_statistic);
        }
    }

    public Data GetData() {
        return new Data(achievements);
    }

    public void LoadData(Data data) {
        foreach (Achievement.Data achievement_data in data.data) {
            if (achievement_dict.ContainsKey(achievement_data.name)) {
                achievement_dict[achievement_data.name].Load(achievement_data);
            }
        }
    }

    public List<Achievement> GetAchievementList() {
        return achievements;
    }

    [System.Serializable]
    public class Data {
        public Achievement.Data[] data;

        public Data(List<Achievement> achievements) {
            data = new Achievement.Data[achievements.Count];
            for (int i = 0; i < achievements.Count; i++) {
                data[i] = achievements[i].Save();
            }
        }
    }
}

[System.Serializable]
public class UnityEventAchievement : UnityEvent<Achievement> {

}