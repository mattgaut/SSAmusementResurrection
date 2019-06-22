using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAccountData : MonoBehaviour {

    public void ResetData() {
        AccountManager.instance.ResetAccount();
    }
}
