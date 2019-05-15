using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    GameObject gameObject { get; }
    Character character { get; }

    CapStat health {
        get;
    }
    float TakeDamage(float damage, ICombatant source);
    void TakeKnockback(ICombatant source, Vector2 force, float length = 0.5f);

    bool is_knocked_back { get; }
    bool invincible { get; }
}

public interface IStats : IDamageable {
    Stat power { get; }
    Stat armor { get; }
    Stat speed { get; }
    CapStat energy { get; }
    Stat knockback_multiplier { get; }
}

public interface ICombatant : IDamageable, IStats {
    float DealDamage(float damage, IDamageable target, bool trigger_on_hit);
    void GiveKnockback(IDamageable target, Vector2 knockback, float duration);
    bool alive {
        get;
    }
    void GiveKillCredit(ICombatant killed);
    void LogBuff(IBuff b);
    Coroutine StartCoroutine(IEnumerator start);
    void DropObject(GameObject obj, bool should_handle_instantiation = false);
}

/// <summary>
/// Interface for interactables that allows interaction with Player
/// </summary>
public interface IInteractable {
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
    Sprite icon { get; }
    bool is_benificial { get; }
    float length { get; }

    void Apply(ICombatant stat_entity);
    void Remove(ICombatant stat_entity);
}

public interface IStatBuff {
    float flat { get; }
    float multi { get; }
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
