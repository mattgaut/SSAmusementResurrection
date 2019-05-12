using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootTablesSingleton : Singleton<LootTablesSingleton> {

    [SerializeField] LootTable _monster_loot, _regular_chest_loot;

    public LootTable monster_loot {
        get { return _monster_loot; }
    }
    public LootTable regular_chest_loot {
        get { return _regular_chest_loot; }
    }
}
