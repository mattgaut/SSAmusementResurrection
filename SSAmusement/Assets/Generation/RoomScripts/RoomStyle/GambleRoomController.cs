using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GambleRoomController : RoomController {
    [SerializeField] RoomSet room_set;

    public override RoomType room_type {
        get { return RoomType.gamble; }
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }
}
