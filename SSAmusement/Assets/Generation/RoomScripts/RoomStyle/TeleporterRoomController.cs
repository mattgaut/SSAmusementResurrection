using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterRoomController : RoomController {

    [SerializeField] RoomSet room_set;

    [SerializeField] Teleporter _teleporter;

    public override RoomType room_type {
        get { return RoomType.teleporter; }
    }

    public Teleporter teleporter {
        get { return _teleporter; }
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }
}
