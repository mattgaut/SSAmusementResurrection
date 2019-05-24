using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class TimedDoor : MonoBehaviour {
    [SerializeField] LayerMask check_if_entered;
    [SerializeField] Door door;
    [SerializeField] TimerDisplay timer_display;

    TimeSpan time_left;

    Func<float> timer;
    bool locked;

    int enter_triggers;
    int seconds_until_closed;

    public void SetTimerDisplay(int time) {
        timer_display.SetTimerDisplay(time);
    }

    public void SetTimer(Func<float> timer_function, int closing_time) {
        timer = timer_function;
        seconds_until_closed = closing_time;
    }

    private void Start() {
        door.SetLocked(false);
        door.Open();
    }

    private void Update() {
        if (timer == null) {
            return;
        }
        time_left = TimeSpan.FromSeconds(seconds_until_closed - timer());
        if (time_left.TotalSeconds >= 0) {
            timer_display.SetTimerDisplay(time_left);
        } else {
            timer_display.SetTimerDisplay(time_left);
        }
        if (!locked && time_left.TotalSeconds <= 0) {
            Lock();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & check_if_entered) != 0) enter_triggers++;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & check_if_entered) != 0) enter_triggers--;
    }

    void Lock() {
        locked = true;
        StartCoroutine(WaitForExit());
    }

    IEnumerator WaitForExit() {
        while (enter_triggers > 0) {
            yield return null;
        }
        door.Close();
        door.SetLocked(true);
    }
}
