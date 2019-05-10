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
