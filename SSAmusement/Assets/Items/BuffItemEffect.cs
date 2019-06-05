using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItemEffect : ItemEffect {

    [SerializeField] BuffGroup buff;

    protected override void OnDrop() {
        buff.GetIBuffInstance().Remove();
    }

    protected override void OnPickup() {
        buff.GetIBuffInstance().Apply(item.owner);
    }
}
