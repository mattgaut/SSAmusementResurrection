using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountCreator : MonoBehaviour {

    [SerializeField] InputField input_field;
    [SerializeField] Button create_button;

    private void Awake() {
        create_button.enabled = false;
        input_field.onValueChanged.AddListener((text) => create_button.enabled = text.Length != 0);
    }

    public void ButtonOnClick() {
        AccountManager.instance.LoadOrCreateAccount(input_field.text);
    }
}
