using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that wraps abilities in a general modifiable manner to allow character to have any 
/// number of defined abilities
/// </summary>
public abstract class AbilitySet : MonoBehaviour {
    protected List<Ability> abilities;
    protected Character character;

    public int count { get { return abilities.Count; } }

    /// <summary>
    /// Checks if ability exists at index
    /// </summary>
    /// <param name="index"></param>
    /// <returns>True if ability exists at index</returns>
    public bool HasAbility(int index) {
        return abilities.Count > index  && index >= 0 && abilities[index] != null;
    }

    /// <summary>
    /// Add ability to end of ability list
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="callback"></param>
    /// <returns>index of ability<returns>
    public int AddAbility(Ability ability) {
        abilities.Add(ability);
        return abilities.Count - 1;
    }

    /// <summary>
    /// Get ability at index
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Ability at index, null if none exists</returns>
    public Ability GetAbility(int index) {
        if (HasAbility(index)) {
            return abilities[index];
        }
        return null;
    }   

    /// <summary>
    /// Get ability by name
    /// </summary>
    /// <param name="index"></param>
    /// <returns>First Ability matching name, null if none exists</returns>
    public Ability GetAbility(string name) {
        for (int i = 0; i < abilities.Count; i++) {
            if (abilities[i].ability_name.ToLower() == name.ToLower()) {
                return abilities[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Get abilities
    /// </summary>
    /// <returns>Ability list</returns>
    public List<Ability> GetAbilities() {
        return abilities;
    }

    /// <summary>
    /// Set Character of all abilities
    /// </summary>
    /// <param name="character"></param>
    public void SetCharacter(Character character) {
        this.character = character;

        foreach (Ability a in abilities) {
            a.SetCharacter(character);
        }
    }

    protected abstract void LoadSkills();

    protected void Awake() {
        abilities = new List<Ability>();

        LoadSkills();
    }
}