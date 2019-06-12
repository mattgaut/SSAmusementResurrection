using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour {

    [SerializeField] Text damage_dealt, damage_taken, kills;

    private void Update() {
        damage_dealt.text = "" + (int)StatTracker.instance.damage_dealt;

        damage_taken.text = "" + (int)StatTracker.instance.damage_taken;

        kills.text = "" + StatTracker.instance.enemies_killed;
    }
}
