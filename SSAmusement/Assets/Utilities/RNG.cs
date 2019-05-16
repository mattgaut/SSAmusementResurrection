using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG {
    static int default_seed;
    static bool is_default_seed_set;

    int seed;
    Random.State state;

    public RNG() {
        if (!is_default_seed_set) {
            default_seed = System.DateTime.Now.Millisecond;
        }
        seed = default_seed;
        default_seed = (default_seed * System.DateTime.Now.Millisecond) % 10000;
        Random.InitState(seed);
        state = Random.state;
    }

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
    /// Returns an int inbetween inclusive lower and inclusive upper bound with an 
    /// average value of avg
    /// </summary>
    /// <param name="lower_bound"></param>
    /// <param name="upper_bound"></param>
    /// <param name="avg"></param>
    /// <returns></returns>
    public int GetInt(int lower_bound, int upper_bound, float avg) {

        float random_value = GetFloat(lower_bound, upper_bound, avg);

        int to_return = (int)random_value;
        to_return += (GetFloat() <= (random_value - to_return)) ? 1 : 0;

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

    /// <summary>
    /// Returns a float value between min and max
    /// </summary>
    /// <param name="lower_bound"></param>
    /// <param name="upper_bound"></param>
    /// <returns>float between lower_bound and upper_bound</returns>
    public float GetFloat(float lower_bound, float upper_bound) {

        Random.state = state;

        float to_return = Random.Range(lower_bound, upper_bound);
        
        state = Random.state;

        return to_return;
    }

    /// <summary>
    /// Returns a float value between min and max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="avg"></param>
    /// <returns>float between lower_bound and upper_bound with an average value of avg</returns>
    public float GetFloat(float min, float max, float avg) {
        float a, b, z;

        a = min;
        b = max - a;
        z = ((max - min) / (avg - min)) - 1f;

        return b * Mathf.Pow(GetFloat(), z) + a;
    }
}
