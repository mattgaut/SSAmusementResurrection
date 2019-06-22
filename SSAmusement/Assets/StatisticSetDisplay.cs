﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticSetDisplay : MonoBehaviour {

    [SerializeField] Transform contianer;
    [SerializeField] StatisticDisplay display;

    [SerializeField] bool is_overall_statistics;

    private void Awake() {
        DisplayStatistics();
    }

    public void DisplayStatistics() {
        for (int i = contianer.childCount - 1; i >= 0; i--) {
            Destroy(contianer.GetChild(i).gameObject);
        }

        IEnumerable<Statistic> stats;
        if (is_overall_statistics) {
            stats = StatisticTrackerManager.instance.GetOverallStatistics();
        } else {
            stats = StatisticTrackerManager.instance.GetRunStatistics();
        }
        foreach (Statistic s in stats) {
            (Instantiate(display, contianer) as StatisticDisplay).Display(s);
        }
    }
}
