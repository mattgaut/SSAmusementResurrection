using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statistic/" + nameof(EnemiesKilled), fileName = nameof(EnemiesKilled), order = 0)]
public class EnemiesKilled : SingleIntStatistic {

    public override SubscriptionTime timing {
        get { return SubscriptionTime.InGame; }
    }

    protected override void OnSubscribe() {
        GameManager.instance.player.on_kill += (a, b) => count++;
    }
}
