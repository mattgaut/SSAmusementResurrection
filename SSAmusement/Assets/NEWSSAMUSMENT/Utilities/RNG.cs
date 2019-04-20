using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG {

    int seed;
    Random.State state;

    public RNG(int seed) {
        this.seed = seed;
        Random.InitState(seed);
        state = Random.state;
    }

    public int GetInt(int lower_bound, int upper_bound) {
        Random.state = state;

        int to_return = Random.Range(lower_bound, upper_bound);

        state = Random.state;

        return to_return;
    }
    public float GetFloat() {
        Random.state = state;

        float to_return = Random.Range(0f, 1f);

        state = Random.state;

        return to_return;
    }
}
