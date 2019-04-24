using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour {

    [SerializeField] MySlider slider;
    [SerializeField] Image ability_image;
    [SerializeField] ActiveAbility _ability;

    Coroutine cooldown_routine;

    public ActiveAbility ability {
        get { return _ability;  }
        private set { _ability = value; }
    }

    public void SetAbility(ActiveAbility ability) {
        this.ability = ability;
        slider.SetFill(0, "");

        ability_image.sprite = ability.icon;
    }

    public void StartCooldown() {
        if (cooldown_routine != null) {
            StopCoroutine(cooldown_routine);
        }
        cooldown_routine = StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown() {
        while (ability.on_cooldown) {
            float time_left = ability.time_until_cooldown_ends / ability.cooldown;
            if (time_left > 1) {
                slider.SetFill(time_left, time_left.ToString("0"));
            } else {
                slider.SetFill(time_left, time_left.ToString("0.0"));
            }

            yield return null;
        }

        slider.SetText("");
        slider.SetFill(0, "");
    }

    private void Awake() {
        if (ability) SetAbility(ability);
    }
}
