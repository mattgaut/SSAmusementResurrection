using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTablesSingleton : Singleton<LootTablesSingleton> {

    [SerializeField] List<PickupChance> possible_pickups;
    
    public List<PickupChance> GetPickupsTable() {
        return new List<PickupChance>(possible_pickups);
    }

    public List<Pickup> GetRolledPickups(RNG rng, float modifier) {
        List<Pickup> to_return = new List<Pickup>();
        foreach (PickupChance p in possible_pickups) {
            if (rng.GetFloat() - modifier < p.chance_per_roll) {
                to_return.Add(p.loot);
            }
        }
        return to_return;
    }
}

[System.Serializable]
public class PickupChance {
    [SerializeField] Pickup _loot;
    [SerializeField] float _chance_per_roll;

    public Pickup loot { get { return _loot; } }
    public float chance_per_roll { get { return _chance_per_roll; } }
}