using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to Inherit from to create new Item Effects
/// </summary>
public abstract class ItemEffect : MonoBehaviour {

    protected Item item { get; set; }

    /// <summary>
    /// Method to define Item Effect behaviour upon being picked up
    /// </summary>
    /// <param name="item">Item Effect belongs to</param>
    public void OnPickup(Item item, int item_count) {
        this.item = item;
        OnPickup();
    }

    protected abstract void OnPickup();

    /// <summary>
    /// Method to define Item Effect behaviour upon being dropped
    /// </summary>
    /// <param name="item">Item effect belongs to</param>
    public void OnDrop(Item item, int item_count) {
        this.item = item;
        OnDrop();
    }

    protected abstract void OnDrop();

}
