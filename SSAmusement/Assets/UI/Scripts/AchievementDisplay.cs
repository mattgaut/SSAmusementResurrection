using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplay : MonoBehaviour {

    [SerializeField] Text display_name, description;
    [SerializeField] Achievement to_display;
    [SerializeField] Slider progress_bar;

    [SerializeField] Image icon;

    [SerializeField] Material unlocked_material, locked_material;

    public void Set(Achievement achievement) {
        display_name.text = achievement.achievement_name;
        description.text = achievement.description;
        
        icon.sprite = achievement.icon;

        if (achievement.is_unlocked) {
            icon.material = unlocked_material;
            progress_bar.value = 1;
        } else {
            icon.material = locked_material;
            progress_bar.value = (achievement.progress / (float)achievement.target);
        }
    }

    private void Awake() {
        Set(to_display);
    }
}
