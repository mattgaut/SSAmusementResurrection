using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : Singleton<AchievementManager> {
    [SerializeField] List<Achievement> achievements;
    [SerializeField] StatisticTrackerManager tracker;

    Dictionary<string, Achievement> achievement_dict;

    protected override void OnAwake() {
        base.OnAwake();

        LoadAchiements(new Data(new List<Achievement>()));
    }

    protected void Start() {

    }

    void LoadAchiements(Data data) {
        achievement_dict = new Dictionary<string, Achievement>();
        foreach (Achievement achievement in achievements) {
            achievement_dict.Add(achievement.achievement_name, achievement);
        }

        foreach (Achievement.Data achievement_data in data.data) {
            if (achievement_dict.ContainsKey(achievement_data.name)) {
                achievement_dict[achievement_data.name].Load(achievement_data);
            }
        }
    }


    [System.Serializable]
    class Data {
        public Achievement.Data[] data;

        public Data(List<Achievement> achievements) {
            data = new Achievement.Data[achievements.Count];
            for (int i = 0; i < achievements.Count; i++) {
                data[i] = achievements[i].Save();
            }
        }
    }
}
