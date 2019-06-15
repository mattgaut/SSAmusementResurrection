using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statistic/" + nameof(DamageDealt), fileName = nameof(DamageDealt), order = 0)]
public class DamageDealt : SingleFloatStatistic {

    public override string name {
        get { return "Damage Dealt"; }
    }

    public override Category category {
        get { return Category.Combat; }
    }

    public override void Subscribe() {
        GameManager.instance.player.on_deal_damage += (a, b, post_mitigation_damage, d) => count += post_mitigation_damage;
    }
}
