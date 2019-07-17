using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItemEffect : ItemEffect {

    [SerializeField] BuffController buff;

    int buff_id;

    protected override void OnFinalDrop() {
        buff.RemoveBuff(buff_id);
    }

    protected override void OnInitialPickup() {
        buff_id = buff.ApplyBuff(item.owner, item.owner);
    }

    protected override void RecalculateEffects() {
        buff.RemoveBuff(buff_id);
        Debug.Log(item.stack_count);
        buff_id = buff.ApplyBuff(item.owner, item.owner, item.stack_count);
    }
}
