using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemDisplay : MonoBehaviour {

    [SerializeField] Image image;
    [SerializeField] Text text;

    public void Display(Item i, int count) {
        if (image == null)
            image = GetComponent<Image>();
        image.sprite = i.icon;
        text.text = "x " + count;
        text.enabled = count > 1;
    }
}
