using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccountButton : MonoBehaviour {

    [SerializeField] Button select_button, delete_button;
    [SerializeField] Text text;

    public void SetText(string text) {
        this.text.text = text;
    }

    public void AddSelectListener(UnityAction action) {
        select_button.onClick.AddListener(action);
    }
    public void AddDeleteListener(UnityAction action) {
        delete_button.onClick.AddListener(action);
    }
}
