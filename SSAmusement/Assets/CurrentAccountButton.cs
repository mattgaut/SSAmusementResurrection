using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentAccountButton : MonoBehaviour {
    [SerializeField] Text display_text;

    public void DisplayName(string name) {
        display_text.text = "Playing as: " + name;
    }

    public void DisplayCurrentAccountName() {
        DisplayName(AccountManager.instance.current_account.name);
    }


    private void Start() {
        DisplayCurrentAccountName();
    }
}
