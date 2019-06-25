using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBuff : MonoBehaviour {

    [SerializeField] Image icon_image, slider_image;
    [SerializeField] Slider slider;
    [SerializeField] Text text;

    IBuff to_display;

    private void Update() {
        if (to_display.length > 0) {
            slider.value = to_display.remaining_time / to_display.length;
        }
        SetText(to_display.stack_count);
        if (!to_display.is_active) {
            Destroy(gameObject);
        }
    }

    public void SetBuff(IBuff buff) {
        to_display = buff;
        icon_image.sprite = buff.icon;
        SetText(buff.stack_count);
        SetBarColor(buff.is_benificial ? Color.green : Color.red);

        if (to_display.length <= 0) {
            slider.value = 0;
        }
    }

    void SetText(string text) {
        this.text.enabled = true;
        this.text.text = text;
    }

    void SetText(int stack_count) {
        text.text = "" + stack_count;
        text.enabled = stack_count > 1;
    }

    void SetBarColor(Color color) {
        slider_image.color = color;
    }
}
