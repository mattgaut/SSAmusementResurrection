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

    /// <summary>
    /// Returns an int in between inclusive lower and exclusive upper bound.
    /// </summary>
    /// <param name="lower_bound"></param>
    /// <param name="upper_bound"></param>
    /// <returns></returns>
    public int GetInt(int lower_bound, int upper_bound) {
        Random.state = state;

        int to_return = Random.Range(lower_bound, upper_bound);

        state = Random.state;

        return to_return;
    }

    /// <summary>
    /// Returns a float value between 0 and 1
    /// </summary>
    /// <returns>float between 0 and 1</returns>
    public float GetFloat() {
        Random.state = state;

        float to_return = Random.Range(0f, 1f);

        state = Random.state;

        return to_return;
    }
}
