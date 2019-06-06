using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour {

    [SerializeField] MySlider health_bar;
    [SerializeField] MySlider energy_bar;
    [SerializeField] GameObject buff_holder;
    [SerializeField] DisplayBuff buff_prefab;
    [SerializeField] GameObject canvas;
    [SerializeField] Text currency_text;
    [SerializeField] Image boss_key_image;

    [SerializeField] AbilityDisplay[] ability_displays;
    [SerializeField] ChargeAbilityDisplay charge_display;

    public void UpdateCurrencyText(int count) {
        currency_text.text = "$" + count;
    }

    public void UpdateHealthBar(float over, float under) {
        health_bar.SetFill(over, under);
    }

    public void UpdateEnergyBar(float over, float under) {
        energy_bar.SetFill(over, under);
    }

    public void UpdateBossKey(int i) {
        if (i > 0) {
            boss_key_image.color = Color.white;
        } else {
            boss_key_image.color = Color.black;
        }
    }

    public void DisplayTimedBuff(IBuff b) {
        DisplayBuff new_display_buff = Instantiate(buff_prefab, buff_holder.transform);
        new_display_buff.SetBuff(b);
    }

    public void Disable() {
        canvas.SetActive(false);
    }

    public void SetAbilityDisplay(ActiveCooldownAbility ability, int i) {
        if (i >= 0 && i < ability_displays.Length) {
            ability_displays[i].SetAbility(ability);
        }
    }

    public void SetActiveItemDisplay(ActiveChargeAbility ability) {
        charge_display.SetAbility(ability);
    }
}
