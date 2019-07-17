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
    public void OnPickup(Item item) {
        this.item = item;
        if (item.stack_count == 1) {
            OnInitialPickup();
        } else {
            OnPickup();
        }
        RecalculateEffects();
    }

    protected abstract void OnInitialPickup();

    protected virtual void OnPickup() { }

    /// <summary>
    /// Method to define Item Effect behaviour upon being dropped
    /// </summary>
    /// <param name="item">Item effect belongs to</param>
    public void OnDrop(Item item) {
        this.item = item;
        if (item.stack_count == 0) {
            OnFinalDrop();
        } else {
            OnDrop();
        }
        RecalculateEffects();
    }

    protected abstract void OnFinalDrop();

    protected virtual void OnDrop() { }

    protected virtual void RecalculateEffects() { }
}
