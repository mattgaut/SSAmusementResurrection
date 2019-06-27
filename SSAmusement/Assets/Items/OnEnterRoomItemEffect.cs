using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnEnterRoomItemEffect : ItemEffect {

    [SerializeField] bool is_first_enter_only;

    protected override void OnDrop() {
        RoomManager.instance.on_enter_room += OnEnterRoom;
    }

    protected override void OnPickup() {
        RoomManager.instance.on_enter_room -= OnEnterRoom;
    }

    protected void OnEnterRoom(RoomController room, bool is_first_enter) {
        if (is_first_enter_only && !is_first_enter) {
            return;
        }
        OnEnterRoomTrigger(room);
    }

    protected abstract void OnEnterRoomTrigger(RoomController room);
}
