using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to Inherit from to create new Item Effects
/// </summary>
public abstract class ItemEffect : MonoBehaviour {

    /// <summary>
    /// Method to define Item Effect behaviour upon being picked up
    /// </summary>
    /// <param name="item">Item Effect belongs to</param>
    public abstract void OnPickup(Item item);


    /// <summary>
    /// Method to define Item Effect behaviour upon being dropped
    /// </summary>
    /// <param name="item">Item effect belongs to</param>
    public abstract void OnDrop(Item item);

}
