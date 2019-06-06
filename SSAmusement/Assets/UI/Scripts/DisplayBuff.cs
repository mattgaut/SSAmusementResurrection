using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBuff : MonoBehaviour {

    [SerializeField] Image icon_image, slider_image;
    [SerializeField] Slider slider;

    IBuff to_display;

    private void Update() {
        slider.value = to_display.remaining_time / to_display.length;
        if (to_display.remaining_time <= 0) {
            Destroy(gameObject);
        }
    }

    public void SetBuff(IBuff buff) {
        to_display = buff;
        icon_image.sprite = buff.icon;
        SetBarColor(buff.is_benificial ? Color.green : Color.red);
    }

    void SetBarColor(Color color) {
        slider_image.color = color;
    }
}
