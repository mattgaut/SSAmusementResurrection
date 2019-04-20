using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory), typeof(PlayerDisplay))]
public abstract class Player : Character, ICombatant {

    Inventory _inventory;
    public Inventory inventory {
        get { return _inventory; }
    }

    public PlayerDisplay player_display { get; private set; }
    public bool can_change_facing {
        get { return true; }
    }

    [SerializeField] Collider2D hitbox;

    public float RestoreHealth(float restore) {
        float old = health.current;
        health.current += restore;
        return health.current - old;
    }

    public override void LogBuff(Buff b) {
        player_display.DisplayTimedBuff(b);
    }

    public void TrackPowerUp(PowerUp p) {
        player_display.DisplayTimedBuff(p);
    }

    protected override void Die() {
        last_hit_by.GiveKillCredit(this);

        alive = false;
        UIHandler.GameOver();
        hitbox.gameObject.SetActive(false);
        anim.SetTrigger("Death");
        anim.SetBool("Flicker", false);
        player_display.Disable();
    }

    protected override IEnumerator IFrames() {
        hitbox.enabled = false;
        anim.SetBool("Flicker", true);
        yield return base.IFrames();
        anim.SetBool("Flicker", false);
        hitbox.enabled = true;
    }

    protected override void OnAwake() {
        _inventory = GetComponent<Inventory>();
        player_display = GetComponent<PlayerDisplay>();
    }

    void Update() {
        player_display.UpdateHealthBar(health.current, health);
    }
}