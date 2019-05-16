using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "ScriptableObjects/Loot Tables")]
public class LootTable : ScriptableObject {
    [SerializeField] List<PickupCategory> possible_pickup_categories;
    [SerializeField] int min_to_drop, max_to_drop;
    [SerializeField] float avg_to_drop;
    
    public void TestNumberDistribution() {
        Dictionary<int, int> numbers = new Dictionary<int, int>(0);
        for (int i = min_to_drop; i <= max_to_drop; i++) {
            numbers.Add(i,0);
        }

        int test_count = 100000;
        RNG rng = new RNG(System.DateTime.Now.Millisecond);
        for (int i = 0; i < test_count; i++) {
            numbers[rng.GetInt(min_to_drop, max_to_drop, avg_to_drop)]++;
        }
        Debug.Log(name);
        int total = 0;
        for (int i = min_to_drop; i <= max_to_drop; i++) {
            total += numbers[i] * i;
            Debug.Log(i + " : " + numbers[i] + " : " + numbers[i]/(test_count / 100f) + "%");
        }
        Debug.Log("Total: " + total);
    }

    public void TestPickupDistribution() {
        Dictionary<Pickup, int> pickups = new Dictionary<Pickup, int>(0);
        int test_count = 1000000;
        RNG rng = new RNG(System.DateTime.Now.Millisecond);
        for (int i = 0; i < test_count; i++) {
            Pickup p = GetRandomPickup(rng, GetRandomCategory(rng));
            if (pickups.ContainsKey(p)) {
                pickups[p]++;
            } else {
                pickups.Add(p, 1);
            }
        }
        Debug.Log(name);
        foreach(Pickup p in pickups.Keys) {
            Debug.Log(p + " : " + pickups[p] + " : " + pickups[p] / (test_count / 100f) + "%");
        }
    }

    /// <summary>
    /// Gets List of pickups of size number_to_roll where the chance of 
    /// a certain pickup being selected for a slot is equal to its chance 
    /// divided by the total chance of all pickups
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="number_to_roll"></param>
    /// <returns></returns>
    public List<Pickup> GetPilePickupes(RNG rng) {
        List<Pickup> pickups = new List<Pickup>();

        int number_to_roll = rng.GetInt(min_to_drop, max_to_drop, avg_to_drop);

        for (int i = 0; i < number_to_roll; i++) {
            pickups.Add(GetRandomPickup(rng, GetRandomCategory(rng)));
        }

        return pickups;
    }

    List<PickupChance> GetRandomCategory(RNG rng) {
        return rng.GetRandomChanceObject(possible_pickup_categories.Cast<IChanceObject<List<PickupChance>>>().ToList());
    }

    Pickup GetRandomPickup(RNG rng, List<PickupChance> possible_pickups) {
        return rng.GetRandomChanceObject(possible_pickups.Cast<IChanceObject<Pickup>>().ToList());
    }

    [System.Serializable]
    class PickupCategory : IChanceObject<List<PickupChance>> {
        public List<PickupChance> possible_pickups;
        [SerializeField] float _chance;

        public List<PickupChance> value {
            get { return new List<PickupChance>(possible_pickups); }
        }
        public float chance {
            get { return _chance; }
        }
    }

    [System.Serializable]
    class PickupChance : IChanceObject<Pickup> {
        public Pickup loot;
        [SerializeField] float _chance;

        public Pickup value {
            get { return loot; }
        }
        public float chance {
            get { return _chance; }
        }
    }
}
