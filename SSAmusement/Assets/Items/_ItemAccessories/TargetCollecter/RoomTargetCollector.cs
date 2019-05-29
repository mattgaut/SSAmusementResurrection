using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTargetCollector : TargetCollector {

    public override Character GetRandomTarget() {
        List<Character> list = GetTargetList();
        if (list.Count > 0) {
            return list.GetRandom();
        }
        return null;
    }

    public override List<Character> GetTargetList() {
        return new List<Character>(RoomManager.instance.active.GetEnemies());
    }
}
