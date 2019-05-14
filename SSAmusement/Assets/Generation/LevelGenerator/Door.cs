using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour {

    Animator anim;

    [SerializeField] bool locked, hard_locked, initially_open, force_open;

    [SerializeField] SFXInfo open_sfx = new SFXInfo("sfx_door_open");
    [SerializeField] SFXInfo close_sfx = new SFXInfo("sfx_door_close");

    private void Awake() {
        anim = GetComponent<Animator>();
        initially_open = force_open || initially_open;
        if (initially_open) {
            Open();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            if (locked) {
                Player p = collision.gameObject.GetComponentInParent<Player>();
                if (p.inventory.TrySpendKeycard()) {
                    locked = false;
                }
            }
            Open();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            Close();
        }
    }

    public void Open() {
        if (!locked && !hard_locked) {
            anim.SetBool("Open", true);
            SoundManager.instance.LocalPlaySfx(open_sfx);
        }
    }
    public void Close() {
        if (!force_open) {
            anim.SetBool("Open", false);
            SoundManager.instance.LocalPlaySfx(close_sfx);
        }
    }

    public void SetLocked(bool _locked) {
        locked = _locked;
        force_open = !locked && force_open;
    }
    public void SetHardLocked(bool _locked) {
        hard_locked = _locked;
        force_open = !hard_locked && force_open;
    }
}
