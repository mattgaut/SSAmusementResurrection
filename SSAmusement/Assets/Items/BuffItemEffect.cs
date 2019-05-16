using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItemEffect : ItemEffect {

    [SerializeField] BuffGroup buff;

    protected override void OnDrop() {
        buff.Remove(item.owner);
    }

    protected override void OnPickup() {
        buff.Apply(item.owner);
    }
}
