using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : Singleton<StatTracker> {

    public int enemies_killed { get; private set; }
    public float damage_taken { get; private set; }
    public float damage_dealt { get; private set; }

    public void StartTracker() {
        enemies_killed = 0;
        damage_taken = 0;
        damage_dealt = 0;

        Player player = GameManager.instance.player;
        player.on_take_damage += (a, pre_mitigation_damage, post_mitigation_damage, d) => damage_taken += post_mitigation_damage;
        player.on_deal_damage += (a, b, post_mitigation_damage, d) => damage_dealt += post_mitigation_damage;
        player.on_kill += (a, b) => enemies_killed++;
    }

    public void EndTracker() {

    }

}
