using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : SingleFloatStatistic {
    public override string name {
        get { return "Damage Taken"; }
    }

    public override void Subscribe() {
        GameManager.instance.player.on_take_damage += (a, b, post_mitigation_damage, d) => count += post_mitigation_damage;
    }
}
