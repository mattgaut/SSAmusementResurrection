using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoomController : RoomController {
    
    public override RoomType room_type { get { return RoomType.basic; } }

    [SerializeField] List<RoomSet> room_sets;

    protected override RoomSet GetRoomsetToLoad() {
        if (room_sets.Count == 0) return null;
        return Instantiate(room_sets[RNGSingleton.instance.room_gen_rng.GetInt(0, room_sets.Count)]);
    }
}