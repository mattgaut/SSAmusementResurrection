using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesPlayed : SingleIntStatistic {

    public override Category category {
        get { return Category.Meta; }
    }

    public override string name {
        get { return "Games Played"; }
    }

    public override void Subscribe() {
        GameManager.instance.on_begin_game += Increment;
    }

    public override void Unsubscribe() {
        GameManager.instance.on_begin_game -= Increment;
    }
}
