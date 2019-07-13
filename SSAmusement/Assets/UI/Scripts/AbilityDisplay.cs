using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour {

    [SerializeField] MySlider slider;
    [SerializeField] Image ability_image;
    [SerializeField] Text charges;
    [SerializeField] GameObject charges_panel;

    Coroutine cooldown_routine;

    public ActiveCooldownAbility ability {
        get; private set;
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

        if (ability.max_charges == 1) {
            charges_panel.SetActive(false);
        } else {
            charges.text = ability.charges + "";
        }
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

            charges_panel.SetActive(ability.max_charges > 1);
            charges.text = ability.charges + "";
        }

        slider.SetText("");
        slider.SetFill(0, "");
    }
}
