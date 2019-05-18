using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeAbilityDisplay : MonoBehaviour {

    [SerializeField] MySlider slider;
    [SerializeField] Image ability_image;

    public ActiveChargeAbility ability {
        get; private set;
    }

    public void SetAbility(ActiveChargeAbility ability) {
        if (this.ability != null) {
            ability.on_charge_changed -= (a, new_number_charges) => SetSlider(new_number_charges);
        }
        if (ability != null) {
            ability_image.sprite = ability.icon;
            ability.on_charge_changed += (a, new_number_charges) => SetSlider(new_number_charges);
        } else {
            ability_image.sprite = null;
        }

        this.ability = ability;
        SetSlider(ability.charges);
    }

    void SetSlider(int number_charges) {
        if (number_charges == ability.cost) {
            slider.SetFill(0, "");
        } else {
            slider.SetFill(ability.cost - number_charges, ability.cost, number_charges + "/" + ability.cost);
        }
    }
}
