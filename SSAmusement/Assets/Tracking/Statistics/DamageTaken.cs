﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statistic/" + nameof(DamageTaken), fileName = nameof(DamageTaken), order = 0)]
public class DamageTaken : SingleFloatStatistic {

    public override SubscriptionTime timing {
        get { return SubscriptionTime.InGame; }
    }

    protected override void OnSubscribe() {
        GameManager.instance.player.on_take_damage += (a, b, post_mitigation_damage, d) => count += post_mitigation_damage;
    }
}
