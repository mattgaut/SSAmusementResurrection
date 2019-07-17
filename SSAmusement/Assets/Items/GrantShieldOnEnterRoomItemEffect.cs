using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantShieldOnEnterRoomItemEffect : ItemEffect {

    bool shields_up;
    int shield_stacks;
    int invincibility;

    [SerializeField] Animator anim;

    void OnEnterRoom(RoomController entered, bool was_first_entry) {
        if (was_first_entry) {
            PutShieldUp();
        } else {
            PutShieldDown();
        }
    }

    void OnWasHit(Character souce, bool hit_successful) {
        if (shields_up) {
            shield_stacks--;
            if (shield_stacks <= 0) {
                PutShieldDown();
            }
        }
    }

    void PutShieldUp() {
        if (shields_up) {
            item.owner.UnlockInvincibility(invincibility);
            item.owner.on_was_hit -= OnWasHit;
        }

        shield_stacks = item.stack_count;
        item.owner.on_was_hit += OnWasHit;
        shields_up = true;
        invincibility = item.owner.LockInvincibility();

        anim.SetBool("IsShieldUp", true);
    }

    void PutShieldDown() {
        item.owner.UnlockInvincibility(invincibility);
        item.owner.on_was_hit -= OnWasHit;
        shields_up = false;
        shield_stacks = 0;

        anim.SetBool("IsShieldUp", false);
    }

    protected override void OnInitialPickup() {
        RoomManager.instance.on_enter_room += OnEnterRoom;
    }

    protected override void OnFinalDrop() {
        RoomManager.instance.on_enter_room -= OnEnterRoom;
        item.owner.on_was_hit -= OnWasHit;
    }
}
