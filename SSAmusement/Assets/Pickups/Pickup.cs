using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour {

    [SerializeField] AudioClip on_pickup_audio_clip;

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
}
