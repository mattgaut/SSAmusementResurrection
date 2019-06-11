using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumeableAbilityDisplay : MonoBehaviour {

    [SerializeField] Image background;
    [SerializeField] GameObject container;

    public void SetDisplay(Consumeable consumeable) {
        background.sprite = consumeable.icon;
        container.SetActive(true);
    }

    public void CloseDisplay() {
        container.SetActive(false);
    }
}
