using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesKilled : SingleIntStatistic {

    public override string name {
        get { return "Enemies Slain"; }
    }

    public override Category category {
        get { return Category.Combat; }
    }

    public override void Subscribe() {
        GameManager.instance.player.on_kill += (a, b) => count++;
    }
}
