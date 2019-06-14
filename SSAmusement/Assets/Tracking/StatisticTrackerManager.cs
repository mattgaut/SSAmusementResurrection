using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StatisticTrackerManager : Singleton<StatisticTrackerManager> {

    static string save_location = "/Stats/stats.ssa";

    [SerializeField] List<Statistic> statistics;

    Dictionary<string, Statistic> overall_statistics_dict;
    Dictionary<string, Statistic> run_statistics_dict;

    public string GetRunStatistic(string stat_name) {
        return run_statistics_dict.ContainsKey(stat_name) ? run_statistics_dict[stat_name].string_value : "";
    }
    public List<Statistic> GetRunStatistics() {
        return new List<Statistic>(run_statistics_dict.Values);
    }

    public string GetOverallStatistic(string stat_name) {
        return overall_statistics_dict.ContainsKey(stat_name) ? overall_statistics_dict[stat_name].string_value : "";
    }
    public List<Statistic> GetOverallStatistics() {
        return new List<Statistic>(overall_statistics_dict.Values);
    }

    public void StartTracker() {
        foreach (Statistic stat in run_statistics_dict.Values) {
            stat.Clear();
            stat.Subscribe();
        }
    }

    public void EndTracker() {
        foreach (Statistic stat in run_statistics_dict.Values) {
            stat.Unsubscribe();
        }
        MergeStatistics();
    }

    protected override void OnAwake() {
        LoadStatistics();
    }

    private void OnDisable() {
        SaveStatistics();
    }

    void LoadStatistics() {
        GameObject run_statistics_object = new GameObject("Run Statistics");
        run_statistics_object.transform.SetParent(transform);

        run_statistics_dict = new Dictionary<string, Statistic>();
        overall_statistics_dict = new Dictionary<string, Statistic>();
        foreach (Statistic stat in statistics) {
            overall_statistics_dict.Add(stat.name, stat);
            if (stat.category == Statistic.Category.Meta) {
                stat.Subscribe();
            } else {
                Statistic run_stat = (Statistic)run_statistics_object.AddComponent(stat.GetType());
                run_statistics_dict.Add(run_stat.name, run_stat);
            }
        }

        if (File.Exists(Application.persistentDataPath + save_location)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + save_location, FileMode.Open);
            StatisticSet stats = (StatisticSet)bf.Deserialize(file);
            file.Close();

            foreach (Statistic.Data data in stats.GetData()) {
                overall_statistics_dict[data.name].Load(data);
            }
        }
    }

    void SaveStatistics() {
        if (!Directory.Exists(Application.persistentDataPath + "/Stats")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/Stats");
        }

        StatisticSet stats_data = new StatisticSet(new List<Statistic>(overall_statistics_dict.Values));

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + save_location);
        bf.Serialize(file, stats_data);
        file.Close();
    }

    void MergeStatistics() {
        foreach (Statistic stat in run_statistics_dict.Values) {
            if (overall_statistics_dict.ContainsKey(stat.name)) {
                overall_statistics_dict[stat.name].TryCombine(stat);
            }
        }
        SaveStatistics();
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
