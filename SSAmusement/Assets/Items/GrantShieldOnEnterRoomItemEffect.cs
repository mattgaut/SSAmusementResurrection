using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantShieldOnEnterRoomItemEffect : ItemEffect {

    bool shields_up;
    int invincibility;

    [SerializeField] Animator anim;

    protected override void OnDrop() {
        item.owner.on_was_hit -= OnWasHit;
    }

    protected override void OnPickup() {
        RoomManager.instance.on_enter_room += OnEnterRoom;
    }

    void OnEnterRoom(RoomController entered, bool was_first_entry) {
        if (was_first_entry) {
            PutShieldUp();
        } else {
            PutShieldDown();
        }
    }

    void OnWasHit(Character souce, bool hit_successful) {
        if (shields_up) {
            PutShieldDown();
        }
    }

    void PutShieldUp() {
        if (shields_up) {
            item.owner.UnlockInvincibility(invincibility);
            item.owner.on_was_hit -= OnWasHit;
        }
        item.owner.on_was_hit += OnWasHit;
        shields_up = true;
        invincibility = item.owner.LockInvincibility();

        anim.SetBool("IsShieldUp", true);
    }

    void PutShieldDown() {
        item.owner.UnlockInvincibility(invincibility);
        item.owner.on_was_hit -= OnWasHit;
        shields_up = false;

        anim.SetBool("IsShieldUp", false);
    }
}
