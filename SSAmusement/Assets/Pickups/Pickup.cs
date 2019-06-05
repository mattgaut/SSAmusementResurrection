using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour {

    [SerializeField] protected SFXInfo on_pickup_audio_clip;

    protected abstract void PickupEffect(Player player);

    protected virtual bool CanPickup(Player p) { return true; }

    bool active = true;
    bool player_inside = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            if (!active) {
                player_inside = true;
            } else {
                Player p = collision.gameObject.GetComponentInParent<Player>();
                if (CanPickup(p)) OnPickup(p);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            player_inside = false;
        }
    }

    protected virtual void OnPickup(Player p) {
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
        active = false;
        coll.enabled = true;
        yield return GameManager.instance.TeamWaitForSeconds(null, time);
        while (player_inside) {
            yield return null;
        }
        active = true;
    }
}
