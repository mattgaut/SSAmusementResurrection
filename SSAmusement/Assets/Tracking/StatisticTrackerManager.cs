using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StatisticTrackerManager : Singleton<StatisticTrackerManager> {

    static string save_location = "/Stats/stats.ssa";

    [SerializeField] List<Statistic> stats_to_track;
    Dictionary<string, Statistic> statistics;

    public string GetStatistic(string stat_name) {
        return statistics.ContainsKey(stat_name) ? statistics[stat_name].string_value : "";
    }
    public List<Statistic> GetStatistics() {
        return new List<Statistic>(statistics.Values);
    }

    public void StartTracker() {
        foreach (Statistic stat in statistics.Values) {
            stat.Subscribe();
        }
    }

    public void EndTracker() {
        foreach (Statistic stat in statistics.Values) {
            stat.Unsubscribe();
        }
        SaveStatistics();
    }

    protected override void OnAwake() {
        LoadStatistics();
    }

    private void OnDisable() {
        SaveStatistics();
    }

    void LoadStatistics() {
        statistics = new Dictionary<string, Statistic>();
        foreach (Statistic stat in stats_to_track) {
            statistics.Add(stat.name, stat);
        }

        if (File.Exists(Application.persistentDataPath + save_location)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + save_location, FileMode.Open);
            StatisticSet stats = (StatisticSet)bf.Deserialize(file);
            file.Close();

            foreach (Statistic.Data data in stats.GetData()) {
                statistics[data.name].Load(data);
            }
        }
    }

    void SaveStatistics() {
        if (!Directory.Exists(Application.persistentDataPath + "/Stats")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/Stats");
        }

        StatisticSet stats_data = new StatisticSet(new List<Statistic>(statistics.Values));

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + save_location);
        bf.Serialize(file, stats_data);
        file.Close();
    }

    [System.Serializable]
    class StatisticSet {
        [SerializeField] List<Statistic.Data> statistics_data;

        public StatisticSet(List<Statistic> statistics) {
            statistics_data = new List<Statistic.Data>();
            foreach (Statistic statistic in statistics) {
                statistics_data.Add(statistic.Save());
            }
        }

        public List<Statistic.Data> GetData() {
            return statistics_data;
        }
    }
}
