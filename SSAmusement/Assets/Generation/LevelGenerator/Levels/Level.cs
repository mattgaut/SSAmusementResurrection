using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject {

    public string level_name {
        get { return _level_name; }
    }

    public ICollection<RoomController> unweighted_rooms {
        get { return _unweighted_rooms; }
    }
    public RoomController spawn_room {
        get { return _spawn_room; }
    }
    public ICollection<BossRoomController> boss_rooms {
        get { return _boss_rooms; }
    }
    public ICollection<TeleporterRoomController> teleporter_rooms {
        get { return _teleporter_rooms; }
    }
    public WeightedRoomGroup shop_rooms {
        get { return _shop_rooms; }
    }
    public WeightedRoomGroup bonus_rooms {
        get { return _bonus_rooms; }
    }
    public WeightedRoomGroup treasure_rooms {
        get { return _treasure_rooms; }
    }
    public List<WeightedRoomGroup> weighted_groups {
        get { return new List<WeightedRoomGroup>() { _shop_rooms, _bonus_rooms, _treasure_rooms }; }
    }
    public LevelAesthetics level_set {
        get { return set; }
    }

    [SerializeField] string _level_name;
    [SerializeField] LevelAesthetics set;
    [SerializeField] RoomController _spawn_room;
    [SerializeField] List<RoomController> _unweighted_rooms;
    [SerializeField] List<BossRoomController> _boss_rooms;
    [SerializeField] List<TeleporterRoomController> _teleporter_rooms;
    [SerializeField] WeightedRoomGroup _shop_rooms;
    [SerializeField] WeightedRoomGroup _bonus_rooms;
    [SerializeField] WeightedRoomGroup _treasure_rooms;

    [System.Serializable]
    public class WeightedRoomGroup {

        public ICollection<RoomController> rooms {
            get { return _rooms; }
        }

        [SerializeField] bool use_range;
        [SerializeField] int fixed_number;
        [SerializeField] int min, max;
        [SerializeField] float avg;
        [SerializeField] List<RoomController> _rooms;

        public int GetNumberToSpawn(RNG rng) {
            if (!use_range) {
                return fixed_number;
            } else {
                return rng.GetInt(min, max, avg);
            }
        }
    }
}
