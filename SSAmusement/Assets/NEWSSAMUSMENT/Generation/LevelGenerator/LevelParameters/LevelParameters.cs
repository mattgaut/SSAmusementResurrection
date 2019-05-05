using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelGenParameters", menuName = "ScriptableObjects/LevelParameters", order = 2)]
public class LevelParameters : ScriptableObject {

    public string level_name {
        get { return _level_name; }
    }

    public IEnumerable<RoomController> unweighted_rooms {
        get { return _unweighted_rooms; }
    }

    public IEnumerable<BossRoomController> boss_rooms {
        get { return _boss_rooms; }
    }

    [SerializeField] string _level_name;
    [SerializeField] LevelSet set;
    [SerializeField] List<RoomController> _unweighted_rooms;
    [SerializeField] List<BossRoomController> _boss_rooms;
    [SerializeField] WeightedRoomGroup _shop_rooms;

    [System.Serializable]
    public class WeightedRoomGroup : ISerializationCallbackReceiver {

        public IEnumerable<RoomController> rooms {
            get { return rooms; }
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
                int to_spawn = (int)formula_value / 1;
                to_spawn += (rng.GetFloat() <= (formula_value % 1f)) ? 1 : 0;
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
