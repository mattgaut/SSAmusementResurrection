using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilitySet))]
public abstract class Ability : MonoBehaviour {

    public enum Type { Active, Passive, Toggle }
    public abstract Type ability_type { get; }

    public virtual ActiveAbility active { get { return null; } }
    //public virtual ToggleAbility toggle { get { return null; } }
    //public virtual PassiveAbility passive { get { return null; } }

    public Character character { get; private set; }

    public Sprite icon { get { return _icon; } }

    public string ability_name { get { return _ability_name; } }

    public virtual bool available { get { return true; } }

    [SerializeField] protected string _ability_name;
    [SerializeField] protected Sprite _icon;

    public virtual void SetCharacter(Character character) {
        this.character = character;
    }
}
