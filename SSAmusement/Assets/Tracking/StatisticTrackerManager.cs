using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StatisticTrackerManager : Singleton<StatisticTrackerManager> {

    static string save_location = "/Stats/stats.ssa";

    [SerializeField] List<Statistic> statistics;

    Dictionary<string, Statistic> overall_statistics_dict;
    Dictionary<string, Statistic> run_statistics_dict;

    bool is_tracking_live_game;

    /// <summary>
    /// Get a statistic that is being tracked for current game.
    /// </summary>
    /// <param name="stat_name">Name of statistic to return</param>
    /// <returns>Named Statistic if it exists</returns>
    public string GetRunStatistic(string stat_name) {
        return run_statistics_dict.ContainsKey(stat_name) ? run_statistics_dict[stat_name].string_value : "";
    }
    public List<Statistic> GetRunStatistics(bool should_include_dynamic_statistics = false) {
        if (!should_include_dynamic_statistics) {
            List<Statistic> to_return = new List<Statistic>();
            foreach (Statistic stat in statistics) {
                if (run_statistics_dict.ContainsKey(stat.name)) {
                    to_return.Add(stat);
                }
            }
            return to_return;
        }
        return new List<Statistic>(run_statistics_dict.Values);
    }

    public string GetOverallStatistic(string stat_name) {
        return overall_statistics_dict.ContainsKey(stat_name) ? overall_statistics_dict[stat_name].string_value : "";
    }
    public List<Statistic> GetOverallStatistics() {
        return new List<Statistic>(overall_statistics_dict.Values);
    }

    /// <summary>
    /// Add Statistic to tracker if one with the same name does not already exist.
    /// Statistics added at runtime are not saved.
    /// </summary>
    /// <param name="stat">Statistic to Add</param>
    public void AddStatistic(Statistic stat) {
        if (overall_statistics_dict.ContainsKey(stat.name) || run_statistics_dict.ContainsKey(stat.name)) {
            return;
        }
        LoadStatistic(stat);
        if (is_tracking_live_game && stat.timing == Statistic.SubscriptionTime.InGame) {
            run_statistics_dict[stat.name].Subscribe();
        }
    }

    /// <summary>
    /// Start Tracker
    /// </summary>
    public void StartTracker() {
        foreach (Statistic stat in run_statistics_dict.Values) {
            stat.Clear();
            stat.Subscribe();
        }
        foreach (Statistic stat in overall_statistics_dict.Values) {
            if (stat.timing == Statistic.SubscriptionTime.InGame) {
                stat.Subscribe();
            }
        }
        is_tracking_live_game = true;
    }

    /// <summary>
    /// End Tracker
    /// </summary>
    public void EndTracker() {
        if (!is_tracking_live_game) {
            return;
        }

        foreach (Statistic stat in run_statistics_dict.Values) {
            stat.Unsubscribe();
        }
        foreach (Statistic stat in overall_statistics_dict.Values) {
            if (stat.timing == Statistic.SubscriptionTime.InGame) {
                stat.Unsubscribe();
            }
        }
        is_tracking_live_game = false;
    }

    protected override void OnAwake() {
        run_statistics_dict = new Dictionary<string, Statistic>();
        overall_statistics_dict = new Dictionary<string, Statistic>();
        foreach (Statistic stat in statistics) {
            LoadStatistic(stat);
        }
    }

    public void LoadData(Data data) {
        foreach (Statistic stat in overall_statistics_dict.Values) {
            stat.Clear();
        }
        foreach (Statistic.Data stat_data in data.GetData()) {
            if (overall_statistics_dict.ContainsKey(stat_data.name)) {
                overall_statistics_dict[stat_data.name].Load(stat_data);
            }
        }
    }

    public Data GetData() {
        return new Data(statistics);
    }

    void LoadStatistic(Statistic stat) {
        if (stat.timing == Statistic.SubscriptionTime.OutOfGame) {
            overall_statistics_dict.Add(stat.name, stat);
            stat.Subscribe();
        } else {
            Statistic run_stat;
            if (stat.is_persistant) {
                overall_statistics_dict.Add(stat.name, stat);
                run_stat = stat.GetNonPersistantClone();
            } else {
                run_stat = stat;
            }
            run_statistics_dict.Add(run_stat.name, run_stat);
        }
    }

    void MergeStatistics() {
        foreach (Statistic stat in run_statistics_dict.Values) {
            if (overall_statistics_dict.ContainsKey(stat.name)) {
                overall_statistics_dict[stat.name].TryCombine(stat);
            }
        }
    }

    [System.Serializable]
    public class Data {
        [SerializeField] List<Statistic.Data> statistics_data;

        public Data(List<Statistic> statistics) {
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
