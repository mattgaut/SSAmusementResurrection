﻿using System.Collections;
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

    [SerializeField] AbilityDisplay[] ability_displays;

    public void UpdateCurrencyText(int count) {
        currency_text.text = "$" + count;
    }

    public void UpdateHealthBar(float over, float under) {
        health_bar.SetFill(over, under);
    }

    public void UpdateEnergyBar(float over, float under) {
        energy_bar.SetFill(over, under);
    }

    public void DisplayTimedBuff(PowerUp p) {
        DisplayBuff new_display_buff = Instantiate(buff_prefab, buff_holder.transform);
        new_display_buff.SetIcon(p.icon);
        new_display_buff.SetTime(p.length);
        new_display_buff.SetBarColor(Color.green);
    }

    public void DisplayTimedBuff(Buff b) {
        DisplayBuff new_display_buff = Instantiate(buff_prefab, buff_holder.transform);
        new_display_buff.SetIcon(b.icon);
        new_display_buff.SetTime(b.buff_length);
        new_display_buff.SetBarColor(b.color);
    }

    public void Disable() {
        canvas.SetActive(false);
    }

    public void SetAbilityDisplay(ActiveAbility ability, int i) {
        if (i >= 0 && i < ability_displays.Length) {
            ability_displays[i].SetAbility(ability);
        }
    }
}
