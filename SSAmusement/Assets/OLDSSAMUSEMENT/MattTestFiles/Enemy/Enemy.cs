﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character, ICombatant {

    [SerializeField] Sprite _icon;
    public Sprite icon {
        get { return _icon; }
    }

    /// <summary>
    /// Room the enemy exists in if any
    /// </summary>
    public Room home {
        get; private set;
    }

    System.Func<IEnumerator> die_function;

    [SerializeField] List<Pickup> drop_on_death;

    /// <summary>
    /// Sets home room
    /// </summary>
    /// <param name="r">Room</param>
    public void SetRoom(Room r) {
        home = r;
    }

    /// <summary>
    /// Add Object to be droped upon death
    /// </summary>
    /// <param name="pickup_prefab_to_drop"></param>
    public void AddDropOnDeath(Pickup pickup_prefab_to_drop) {
        drop_on_death.Add(pickup_prefab_to_drop);
    }

    /// <summary>
    /// Set Callback to be invoked on death before object is destroyed
    /// </summary>
    /// <param name="die_event"></param>
    public void SetDieEvent(System.Func<IEnumerator> die_event) {
        die_function = die_event;
    }

    protected override void Die() {
        last_hit_by.GiveKillCredit(this);

        if (die_function != null) {
            StartCoroutine(BeforeDestroy());
        } else {
            OnDie();
        }
        alive = false;
    }

    void OnDie() {
        DropPickups();
        if (home) home.RemoveEnemy(this);
        Destroy(gameObject);
    }

    void DropPickups() {
        foreach (Pickup pickup in drop_on_death) {
            DropObject(pickup.gameObject, true);
        }
    }

    IEnumerator BeforeDestroy() {
        yield return die_function();
        OnDie();
    }
}
