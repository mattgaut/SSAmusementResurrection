using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementUIHandler : MonoBehaviour {

    [SerializeField] AchievementDisplay display_prefab;
    [SerializeField] Transform container;

    public void DisplayAchievements(IEnumerable<Achievement> achievements) {
        for (int i = container.childCount - 1; i >= 0; i--) {
            Destroy(container.GetChild(i).gameObject);
        }

        foreach (Achievement achievement in achievements) {
            AchievementDisplay new_display = Instantiate(display_prefab, container);
            new_display.Set(achievement);
        }
    }

    private void OnEnable() {
        if (AchievementManager.has_instance) {
            DisplayAchievements(AchievementManager.instance.GetAchievementList());
        }
    }
}
