using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAbilityEffect : MonoBehaviour {

    public bool is_using_ability { get; protected set; }

    protected Character character { get; private set; }

    public void UseAbilityEffect(Character character, float input) {
        SetCharacter(character);
        UseAbilityEffect(input);
    }

    public virtual void SetCharacter(Character character) {
        this.character = character;
    }

    protected abstract void UseAbilityEffect(float input);

    protected void OnDisable() {
        is_using_ability = false;
    }
}
