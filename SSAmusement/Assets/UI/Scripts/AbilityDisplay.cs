using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour {

    [SerializeField] MySlider slider;
    [SerializeField] Image ability_image;
    [SerializeField] ActiveCooldownAbility _ability;

    Coroutine cooldown_routine;

    public ActiveCooldownAbility ability {
        get { return _ability;  }
        private set { _ability = value; }
    }

    public void SetAbility(ActiveCooldownAbility ability) {
        if (ability != null) {
            ability_image.sprite = ability.icon;
        } else {
            ability_image.sprite = null;
        }

        this.ability = ability;
        ability.on_ability_used.AddListener(StartCooldown);
        slider.SetFill(0, "");
    }

    public void StartCooldown() {
        if (cooldown_routine != null) {
            StopCoroutine(cooldown_routine);
        }
        cooldown_routine = StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown() {
        while (ability.is_on_cooldown) {
            float time_left = ability.time_until_cooldown_ends;
            if (time_left > 1) {
                slider.SetFill(time_left, ability.cooldown, time_left.ToString("0"));
            } else {
                slider.SetFill(time_left, ability.cooldown, time_left.ToString("0.0"));
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
