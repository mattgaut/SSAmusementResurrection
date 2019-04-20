using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {
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