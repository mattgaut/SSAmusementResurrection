using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAbilityEffect : MonoBehaviour {

    public bool is_using_ability { get; protected set; }

    protected Character character { get; private set; }

    public void TriggerEffect(Character character, float input) {
        SetCharacter(character);
        UseAbility(input);
    }

    public virtual void SetCharacter(Character character) {
        this.character = character;
    }

    protected abstract void UseAbility(float input); 
}
