using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItemEffect : ItemEffect {

    [SerializeField] BuffController buff;

    int buff_id;

    protected override void OnDrop() {
        buff.RemoveBuff(buff_id);
    }

    protected override void OnPickup() {
        buff_id = buff.ApplyBuff(item.owner);
    }
}
