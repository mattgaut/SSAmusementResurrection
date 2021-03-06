﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for interactables that allows interaction with Player
/// </summary>
public interface IInteractable {
    bool is_available { get; }

    void Interact(Player player);

    void SetHighlight(bool is_highlighted);
}

/// <summary>
/// Interface to generalize and allow access to what inputs characters are 
/// recieving from their ai or player controllers
/// </summary>
public interface IInputHandler {
    Vector2 input { get; }
    int facing { get; }

    event Action<bool> on_jump;
    event Action on_land;
}

public interface IBuff {
    Character buffed { get; }
    Character source { get; }
    Sprite icon { get; }
    bool is_benificial { get; }
    bool is_active { get; }
    float length { get; }
    float remaining_time { get; }
    int stack_count { get; }

    void Apply(Character to_buff, Character source);
    void Remove();

    void AddStack();
    void RemoveStack();
    void AddStack(int i);
    void RemoveStack(int i);
    void SetStacks(int i);
}

/// <summary>
/// Interface that defines an object in a loot table
/// allowing use of the RNG class to pick one from a list at random
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IChanceObject<T> {
    T value { get; }
    float chance { get; }
}
