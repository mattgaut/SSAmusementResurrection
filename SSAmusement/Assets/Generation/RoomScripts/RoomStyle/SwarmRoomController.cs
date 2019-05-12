using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmRoomController : RoomController {

    [SerializeField] EnemySwarmInteractable swarm_button;
    [SerializeField] RoomSet room_set;

    public override RoomType room_type {
        get {
            return RoomType.swarm;
        }
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }

    public override void Init() {
        base.Init();
        swarm_button.Init(room);
    }
}
