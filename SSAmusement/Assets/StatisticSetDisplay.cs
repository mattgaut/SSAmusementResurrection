using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticSetDisplay : MonoBehaviour {

    [SerializeField] Transform contianer;
    [SerializeField] StatisticDisplay display;

    private void Awake() {
        foreach (Statistic s in StatisticTrackerManager.instance.GetStatistics()) {
            (Instantiate(display, contianer) as StatisticDisplay).Display(s);
        }
    }
}
