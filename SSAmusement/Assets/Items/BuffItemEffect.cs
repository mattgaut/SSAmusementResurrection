using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItemEffect : ItemEffect {

    [SerializeField] BuffGroup buff;

    public override void OnDrop(Item item) {
        buff.Remove(item.owner);
    }

    public override void OnPickup(Item item) {
        buff.Apply(item.owner);
    }
}
