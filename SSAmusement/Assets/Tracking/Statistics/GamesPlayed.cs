using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statistic/" + nameof(GamesPlayed), fileName = nameof(GamesPlayed), order = 0)]
public class GamesPlayed : SingleIntStatistic {

    public override Category category {
        get { return Category.Meta; }
    }

    public override string name {
        get { return "Games Played"; }
    }

    protected override void OnSubscribe() {
        GameManager.instance.on_begin_game += Increment;
    }

    protected override void OnUnsubscribe() {
        GameManager.instance.on_begin_game -= Increment;
    }
}
