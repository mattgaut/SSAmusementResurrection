﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour {

    [SerializeField] SFXInfo on_pickup_audio_clip;

    bool picked_up = false;

    protected abstract void PickupEffect(Player player);

    protected virtual bool CanPickup(Player p) { return true; }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            Player p = collision.gameObject.GetComponentInParent<Player>();
            if (CanPickup(p)) OnPickup(p);
        }
    }

    void OnPickup(Player p) {
        picked_up = true;

        PickupEffect(p);

        SoundManager.instance?.LocalPlaySfx(on_pickup_audio_clip);

        Destroy(gameObject);
    }

    private void Awake() {
        Collider2D coll = GetComponent<Collider2D>();
        coll.enabled = false;

        StartCoroutine(EnablePickupAfterTime(coll, .5f));
    }
    
    IEnumerator EnablePickupAfterTime(Collider2D coll, float time) {
        yield return GameManager.instance.TeamWaitForSeconds(null, time);
        coll.enabled = true;
    }
}
