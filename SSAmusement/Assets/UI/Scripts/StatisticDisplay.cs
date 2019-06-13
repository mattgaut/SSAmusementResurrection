using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticDisplay : MonoBehaviour {
    [SerializeField] Text title, value;

    public void Display(Statistic stat) {
        title.text = stat.name;
        value.text = stat.string_value;
    }
}
