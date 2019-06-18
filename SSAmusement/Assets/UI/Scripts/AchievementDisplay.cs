using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplay : MonoBehaviour {

    [SerializeField] Text display_name, description;
    [SerializeField] Achievement to_display;

    public void Set(Achievement achievement) {
        display_name.text = achievement.achievement_name;
        description.text = achievement.description;

        Color color = (achievement.is_unlocked ? Color.green : Color.red);

        display_name.color = color;
        description.color = color;
    }

    private void Awake() {
        Set(to_display);
    }
}
