using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public WeightedRoomGroup swarm_rooms {
        get { return _swarm_rooms; }
    }
    public List<WeightedRoomGroup> weighted_groups {
        get { return new List<WeightedRoomGroup>() { shop_rooms, _swarm_rooms }; }
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
    [SerializeField] WeightedRoomGroup _swarm_rooms;

    [System.Serializable]
    public class WeightedRoomGroup : ISerializationCallbackReceiver {

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
                if (min > max) {
                    Debug.LogError("Min value above Max value");
                }
                if (avg <= min || avg >= max) {
                    Debug.LogError("Avg value outside range");
                }

                float rng_value = rng.GetFloat();
                float formula_value = b * Mathf.Pow(rng_value, z) + a;
                int to_spawn = (int)formula_value;
                to_spawn += (rng.GetFloat() <= (formula_value - to_spawn)) ? 1 : 0;
                return to_spawn;
            }
        }

        float a, b, c, z;

        void Awake() {
            if (use_range) {
                a = min;
                b = max - a;
                c = avg;
                z = ((max - min) / (avg - min)) - 1f;
            }
        }

        public void OnBeforeSerialize() {
           
        }

        public void OnAfterDeserialize() {
            Awake();
        }
    }
}
