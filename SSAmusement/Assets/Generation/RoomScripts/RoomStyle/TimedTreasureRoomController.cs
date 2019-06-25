using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTreasureRoomController : RoomController {
    [SerializeField] RoomSet room_set;

    [SerializeField] List<TimedDoor> timed_doors;
    [SerializeField] List<ItemChest> rewards;

    [SerializeField] int seconds_per_level;
    [SerializeField] int base_seconds;

    public override RoomType room_type {
        get { return RoomType.treasure; }
    }

    public override void Init() {
        base.Init();
        int door_count = 0;
        foreach (TimedDoor timed_door in timed_doors) {
            timed_door.SetTimer(() => GameManager.instance.game_time, base_seconds + ((GameManager.instance.level_count - 1) * seconds_per_level) + (door_count++ * 15));
        }
        foreach (ItemChest chest in rewards) {
            chest.SetSpawnItem(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.item_rng));
        }
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }
}
