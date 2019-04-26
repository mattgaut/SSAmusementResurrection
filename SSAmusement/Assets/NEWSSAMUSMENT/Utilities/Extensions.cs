using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {

    /// <summary>
    /// Shuffles a list using the Knuth\Fisher-Yates shuffle method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(this List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    /// <summary>
    /// Shuffles a list using the Knuth\Fisher-Yates shuffle method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="rng">RNG instance to use</param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(this List<T> list, RNG rng) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.GetInt(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    /// <summary>
    /// Moves object from one index to another
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index_from"></param>
    /// <param name="index_to"></param>
    /// <returns>bool that corresponds to whether the move was performed</returns>
    public static bool Move<T>(this List<T> list, int index_from, int index_to) {
        if (list.Count <= index_from) {
            return false;
        }

        T to_move = list[index_from];
        list.RemoveAt(index_from);
        list.Insert(index_to, to_move);

        return true;
    }
}