using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statistic/" + nameof(EnemiesKilled), fileName = nameof(EnemiesKilled), order = 0)]
public class EnemiesKilled : SingleIntStatistic {

    public override string name {
        get { return "Enemies Slain"; }
    }

    public override Category category {
        get { return Category.Combat; }
    }

    protected override void OnSubscribe() {
        GameManager.instance.player.on_kill += (a, b) => count++;
    }
}
