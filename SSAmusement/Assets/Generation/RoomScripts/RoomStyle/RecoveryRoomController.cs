using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryRoomController : RoomController {

    public override RoomType room_type {
        get { return RoomType.recovery; }
    }
    protected override RoomSet GetRoomsetToLoad() {
        return null;
    }
}
