using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public Sprite icon {
        get { return _icon; }
    }

    /// <summary>
    /// Room the enemy exists in if any
    /// </summary>
    public RoomController home {
        get; private set;
    }

    System.Func<IEnumerator> die_function;

    [SerializeField] Sprite _icon;
    [SerializeField] List<Pickup> drop_on_death;

    /// <summary>
    /// Sets home room
    /// </summary>
    /// <param name="r">Room Controller</param>
    public void SetHome(RoomController r) {
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
    /// Add Objects to be droped upon death
    /// </summary>
    /// <param name="pickup_prefab_to_drop"></param>
    public void AddDropOnDeath(List<Pickup> pickup_prefab_to_drop) {
        drop_on_death.AddRange(pickup_prefab_to_drop);
    }

    /// <summary>
    /// Set Callback to be invoked on death before object is destroyed
    /// </summary>
    /// <param name="die_event"></param>
    public void SetDieEvent(System.Func<IEnumerator> die_event) {
        die_function = die_event;
    }

    protected override void Die(Character killed_by) {
        last_hit_by.GiveKillCredit(this);
        InvokeOnDeath(this, killed_by);
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
        GameManager.instance.RemoveOnTimeScaleChangedEvent(team, OnTimeScaleChanged);
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
