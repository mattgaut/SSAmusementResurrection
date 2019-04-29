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

    Vector2 knockback_force { get; }
    bool is_knocked_back { get; }
    bool invincible { get; }
}

public interface IStats : IDamageable {
    Stat power { get; }
    Stat armor { get; }
    Stat speed { get; }
    CapStat energy { get; }
}

public interface ICombatant : IDamageable, IStats {
    float DealDamage(float damage, IDamageable target, bool trigger_on_hit);
    bool alive {
        get;
    }
    void GiveKillCredit(ICombatant killed);
    void LogBuff(Buff b);
    Coroutine StartCoroutine(IEnumerator start);
    void DropObject(GameObject obj, bool should_handle_instantiation = false);
}

/// <summary>
/// Interface for interactables that allows interaction with Player
/// </summary>
public interface IInteractable {
    void Interact(Player player);
}

/// <summary>
/// Interface to generalize and allow access to what inputs characters are 
/// recieving from their ai or player controllers
/// </summary>
public interface IInputHandler {
    Vector2 input { get; }
    int facing { get; }
}
