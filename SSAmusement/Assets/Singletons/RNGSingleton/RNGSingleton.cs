﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNGSingleton : Singleton<RNGSingleton> {

    [SerializeField] bool use_seed;
    [SerializeField] int seed;

    public RNG item_rng { get; private set; }
    public RNG random_item_rng { get; private set; }
    public RNG room_gen_rng { get; private set; }
    public RNG loot_rng { get; private set; }
    public RNG swarm_rng { get; private set; }

    protected override void OnAwake() {
        if (!use_seed) {
            seed = System.DateTime.Now.Millisecond;
        }
        SetSeed(seed);
    }

    public void SetSeed(int seed) {
        item_rng = new RNG(seed);
        random_item_rng = new RNG(seed - 1);
        room_gen_rng = new RNG(seed + 1);
        loot_rng = new RNG(seed + 2);
        swarm_rng = new RNG(seed + 3);

        this.seed = seed;
    }

    public void ResetSeed() {
        SetSeed(seed);
    }
}
