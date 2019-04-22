using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : MonoBehaviour {

    public abstract void OnPickup(Item i);

    public abstract void OnDrop(Item i);

}
