using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class PowerUp : Pickup, IBuff {

    public Sprite icon {
        get { return _icon; }
    }
    public abstract float length {
        get;
    }
    public bool is_benificial {
        get { return _is_benificial; }
    }

    [SerializeField] bool _is_benificial;
    Sprite _icon;
    protected float timer;
    protected bool countdown;

    SpriteRenderer sr;

    Collider2D coll;

    protected virtual void BeforeDestroy() {

    }

    protected override void PickupEffect(Player player) {
        AddPowerup(player);
    }

    protected virtual void AddPowerup(Player p) {
        p.LogBuff(this);
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
