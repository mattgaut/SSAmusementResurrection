using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class Player : Character {

    public Inventory inventory { get; private set; }
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

    [SerializeField] Collider2D hitbox;
    [SerializeField] PlayerDisplay _player_display;

    /// <summary>
    /// Adds buff timer to PlayerDisplay
    /// </summary>
    /// <param name="b">Buff</param>
    public override void LogBuff(IBuff b) {
        player_display.DisplayBuff(b);
    }

    protected override void Die(Character killed_by) {
        is_alive = false;

        last_hit_by.GiveKillCredit(this);
        GameManager.instance.RemoveOnTimeScaleChangedEvent(team, OnTimeScaleChanged);
        OnTimeScaleChanged(1f);
        InvokeOnDeath(this, killed_by);

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

        inventory = GetComponent<Inventory>();
    }

    void Update() {
        player_display.UpdateHealthBar(health.current, health.max);
        player_display.UpdateEnergyBar(energy.current, energy.max);
    }

}