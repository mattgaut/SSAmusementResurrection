using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTablesSingleton : Singleton<LootTablesSingleton> {

    [SerializeField] List<PickupChance> possible_pickups;
    
    public List<PickupChance> GetPickupsTable() {
        return new List<PickupChance>(possible_pickups);
    }


    /// <summary>
    /// Gets list of pickups where each pickup individually
    /// had a chance to be present according to its specific chance
    /// </summary>
    /// <param name="rng"></param>
    /// <returns>List of Pickups</returns>
    public List<Pickup> GetRolledPickups(RNG rng) {
        List<Pickup> to_return = new List<Pickup>();
        foreach (PickupChance p in possible_pickups) {
            if (rng.GetFloat() < p.chance_per_roll) {
                to_return.Add(p.loot);
            }
        }
        return to_return;
    }

    /// <summary>
    /// Gets List of pickups of size number_to_roll where the chance of 
    /// a certain pickup being selected for a slot is equal to its chance 
    /// divided by the total chance of all pickups
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="number_to_roll"></param>
    /// <returns></returns>
    public List<Pickup> GetPilePickupes(RNG rng, int number_to_roll) {
        List<Pickup> pickups = new List<Pickup>();

        float total_chance = 0;
        foreach (PickupChance pc in possible_pickups) {
            total_chance += pc.chance_per_roll;
        }

        for (int i = 0; i < number_to_roll; i++) {
            float roll = rng.GetFloat(0, total_chance);
            foreach (PickupChance pc in possible_pickups) {
                roll -= pc.chance_per_roll;
                if (roll <= 0) {
                    pickups.Add(pc.loot);
                    break;
                }
            }
        }

        return pickups;
    }
}

[System.Serializable]
public class PickupChance {
    [SerializeField] Pickup _loot;
    [SerializeField] float _chance_per_roll;

    public Pickup loot { get { return _loot; } }
    public float chance_per_roll { get { return _chance_per_roll; } }
}