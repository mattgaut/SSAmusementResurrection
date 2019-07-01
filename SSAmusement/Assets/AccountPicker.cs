using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccountPicker : MonoBehaviour {

    [SerializeField] AccountButton button_prefab;

    [SerializeField] Transform content;

    [SerializeField] UnityEventString on_account_picked, on_account_deleted;

    List<AccountButton> buttons;

    private void OnEnable() {
        buttons = new List<AccountButton>();
        foreach (string account in AccountManager.instance.GetAccounts()) {
            string current_account_name = account;
            AccountButton new_button = Instantiate(button_prefab, content);
            new_button.GetComponentInChildren<Text>().text = current_account_name;
            new_button.AddSelectListener(() => PickAccount(current_account_name));
            new_button.AddDeleteListener(() => DeleteAccount(current_account_name, new_button));
            buttons.Add(new_button);
        }
    }

    private void OnDisable() {
        foreach (AccountButton button in buttons) {
            Destroy(button.gameObject);
        }
    }

    void PickAccount(string name) {
        AccountManager.instance.LoadAccount(name);
        on_account_picked?.Invoke(name);
    }

    void DeleteAccount(string name, AccountButton button) {
        if (AccountManager.instance.GetAccounts().Length > 1) {
            AccountManager.instance.DeleteAccount(name);
            buttons.Remove(button);
            Destroy(button.gameObject);
            on_account_deleted?.Invoke(name);
        }
    }
}

[System.Serializable]
public class UnityEventString : UnityEvent<string> {

}
