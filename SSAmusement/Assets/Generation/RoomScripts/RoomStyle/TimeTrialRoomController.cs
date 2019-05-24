using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialRoomController : RoomController {

    [SerializeField] RoomSet room_set;

    [SerializeField] TimedDoor timed_door;

    [SerializeField] Transform[] chest_spawns;

    [SerializeField] PickupChest pickup_chest_prefab;
    [SerializeField] ItemChest item_chest_prefab;

    [SerializeField] int countdown_length;

    bool has_been_attempted = false;

    public override RoomType room_type {
        get { return RoomType.treasure; }
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }

    public void StartCountDown() {
        if (!has_been_attempted) {
            float current_game_time = GameManager.instance.game_time;
            timed_door.SetTimer(() => GameManager.instance.game_time - current_game_time, countdown_length);
            timed_door.enabled = true;
            has_been_attempted = true;
        }
    }

    public override void Init() {
        base.Init();

        foreach (Transform t in chest_spawns) {
            if (RNGSingleton.instance.room_gen_rng.GetFloat() < 0.67) {
                PickupChest new_chest = Instantiate(pickup_chest_prefab);
                new_chest.transform.position = t.position;
                new_chest.SetSpawnPickups(LootTablesSingleton.instance.regular_chest_loot.GetPilePickups(RNGSingleton.instance.loot_rng));
                new_chest.SetOpenToPlayer(true);
            } else {
                ItemChest new_chest = Instantiate(item_chest_prefab);
                new_chest.transform.position = t.position;
                new_chest.SetSpawnItem(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.item_rng));
            }
        }
        timed_door.enabled = false;
        timed_door.SetTimerDisplay(countdown_length);
    }


}
