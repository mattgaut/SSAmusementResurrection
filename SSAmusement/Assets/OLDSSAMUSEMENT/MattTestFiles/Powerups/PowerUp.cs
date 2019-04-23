using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class PowerUp : Pickup {

    Sprite _icon;
    public Sprite icon {
        get { return _icon; }
    }

    protected float timer;
    protected bool countdown;

    SpriteRenderer sr;

    Collider2D coll;

    public abstract float length {
        get;
    }

    protected virtual void BeforeDestroy() {

    }

    protected override void PickupEffect(Player player) {
        AddPowerup(player);
    }

    protected virtual void AddPowerup(Player p) {
        p.TrackPowerUp(this);
        coll.enabled = false;
        sr.enabled = false;
        countdown = true;

        timer = length;
    }

    private void Update() {
        if (countdown) {
            timer -= Time.deltaTime;
            if (timer < 0) {
                BeforeDestroy();
                Destroy(gameObject);
            }
        }
    }

    private void Awake() {
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        _icon = sr.sprite;
    }

}
