using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player : Character, ICombatant {

    public Inventory inventory {
        get { return _inventory; }
    }
    public PlayerDisplay player_display {
        get { return _player_display; }
    }

    /// <summary>
    /// Can the player turn around?
    /// </summary>
    public bool can_change_facing {
        get { return true; }
    }

    public virtual bool can_use_basic_ability {
        get { return true; }
    }
    public virtual bool can_use_skill_1 {
        get { return true; }
    }
    public virtual bool can_use_skill_2 {
        get { return true; }
    }
    public virtual bool can_use_skill_3 {
        get { return true; }
    }

    Inventory _inventory;
    [SerializeField] Collider2D hitbox;
    [SerializeField] PlayerDisplay _player_display;


    /// <summary>
    /// Restore Health to the player
    /// </summary>
    /// <param name="restore">Amount of health to restore</param>
    /// <returns>Total health restored</returns>
    public float RestoreHealth(float restore) {
        float old = health.current;
        health.current += restore;
        return health.current - old;
    }

    /// <summary>
    /// Restore Energy to the player
    /// </summary>
    /// <param name="restore">Amount of Energy to restore</param>
    /// <returns>Total Energy restored</returns>
    public float RestoreEnergy(float restore) {
        float old = energy.current;
        energy.current += restore;
        return energy.current - old;
    }

    /// <summary>
    /// Adds buff timer to PlayerDisplay
    /// </summary>
    /// <param name="b">Buff</param>
    public override void LogBuff(IBuff b) {
        player_display.DisplayTimedBuff(b);
    }

    protected override void Die(ICombatant killed_by) {
        last_hit_by.GiveKillCredit(this);
        foreach (OnDeathCallback ocd in on_deaths) {
            ocd.Invoke(this, killed_by);
        }

        alive = false;
        GameManager.instance.GameOver();
        hitbox.gameObject.SetActive(false);
        anim.SetTrigger("Death");
        anim.SetBool("Flicker", false);
        player_display.Disable();
    }

    protected override IEnumerator IFrames() {
        anim.SetBool("Flicker", true);
        yield return base.IFrames();
        anim.SetBool("Flicker", false);
    }

    protected override void OnAwake() {
        base.OnAwake();

        _inventory = GetComponent<Inventory>();
    }

    void Update() {
        player_display.UpdateHealthBar(health.current, health.max);
        player_display.UpdateEnergyBar(energy.current, energy.max);
    }
}