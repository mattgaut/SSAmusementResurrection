using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCollected : SingleIntStatistic {
    public override string name {
        get { return "Items Collected"; }
    }

    public override void Subscribe() {
        GameManager.instance.player.inventory.on_collect_item += (item) => count++;
    }
}
