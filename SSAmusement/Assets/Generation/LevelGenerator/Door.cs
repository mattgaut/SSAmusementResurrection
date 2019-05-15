using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour {

    Animator anim;

    [SerializeField] bool locked, hard_locked, initially_open, force_open;

    [SerializeField] SFXInfo open_sfx = new SFXInfo("sfx_door_open");
    [SerializeField] SFXInfo close_sfx = new SFXInfo("sfx_door_close");

    Coroutine wait_open, wait_close;

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

    public void SetOpen(bool is_open) {
        if (is_open) {
            Open();
        } else {
            Close();
        }
    }

    public void Open() {
        if (!locked && !hard_locked) {
            anim.SetBool("Open", true);
            if (wait_open == null) wait_open = StartCoroutine(WaitForOpen());
        }
    }
    public void Close() {
        if (!force_open) {
            anim.SetBool("Open", false);
            if (wait_close == null) wait_close = StartCoroutine(WaitForClose());
        }
    }

    IEnumerator WaitForOpen() {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("DoorClose")) {
            yield return null;
        }
        if (!RoomManager.has_instance || RoomManager.instance.IsInActiveRoom(transform.position)) {
            SoundManager.instance.LocalPlaySfx(open_sfx);
        }
        wait_open = null;
    }
    IEnumerator WaitForClose() {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("DoorOpen")) {
            yield return null;
        }
        if (!RoomManager.has_instance || RoomManager.instance.IsInActiveRoom(transform.position)) {
            SoundManager.instance.LocalPlaySfx(close_sfx);
        }
        wait_close = null;
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
