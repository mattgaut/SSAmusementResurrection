using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Formula {
    [SerializeField] float flat_damage;
    [SerializeField] float power_ratio;

    public float GetValue(Character character) {
        return flat_damage + (power_ratio * character.power);
    }
}
