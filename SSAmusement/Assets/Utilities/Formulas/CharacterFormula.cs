using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(order = 10, menuName = "ScriptableObjects/Formulas/CharacterFormula", fileName = "CharacterFormula")]
public class CharacterFormula : Formulas.Formula<Character> {

    [SerializeField] bool has_flat;
    [SerializeField] float flat;
    [SerializeField] bool has_power;
    [SerializeField] float power_modifier;
    [SerializeField] bool has_speed;
    [SerializeField] float speed_modifier;
    [SerializeField] bool has_armor;
    [SerializeField] float armor_modifier;
    [SerializeField] bool has_attack_speed;
    [SerializeField] float attack_speed_modifier;


    List<Func<Character, float>> formulas;

    public override float GetValue(Character character) {
        float result = 0;
        foreach (Func<Character, float> formula in formulas) {
            result += formula(character);
        }
        return result;
    }

    private void OnEnable() {
        formulas = new List<Func<Character, float>>();
        if (!has_flat) {
            flat = 0;
        }
        formulas.Add(new Func<Character, float>((character) => flat));
        if (has_power) {
            formulas.Add(new Func<Character, float>((character) => character.power * power_modifier));
        }
        if (has_speed) {
            formulas.Add(new Func<Character, float>((character) => character.speed * speed_modifier));
        }
        if (has_armor) {
            formulas.Add(new Func<Character, float>((character) => character.armor * armor_modifier));
        }
        if (has_attack_speed) {
            formulas.Add(new Func<Character, float>((character) => character.stats.attack_speed * attack_speed_modifier));
        }
    }
}
