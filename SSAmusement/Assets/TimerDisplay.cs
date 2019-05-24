using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour {

    [SerializeField] Text timer_display;
    [SerializeField] TimerDisplay parent_display;

    public void SetTimerDisplay(int time) {
        TimeSpan ts = new TimeSpan(0, 0, 0, time);
        SetTimerDisplay(ts);
    }
    public void SetTimerDisplay(TimeSpan time) {
        if (time.TotalSeconds < 0) {
            timer_display.text = "XX:XX";
        } else {
            timer_display.text = $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        }        
    }

    private void LateUpdate() {
        if (parent_display != null) {
            timer_display.text = parent_display.timer_display.text;
        }        
    }
}
