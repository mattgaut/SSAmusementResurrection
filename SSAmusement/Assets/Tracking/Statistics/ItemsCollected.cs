using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Statistic/" + nameof(ItemsCollected), fileName = nameof(ItemsCollected), order = 0)]
public class ItemsCollected : SingleIntStatistic {
    public override SubscriptionTime timing {
        get { return SubscriptionTime.InGame; }
    }

    protected override void OnSubscribe() {
        GameManager.instance.player.inventory.on_collect_item += (item) => count++;
    }
}
