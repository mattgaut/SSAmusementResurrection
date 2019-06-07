using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveAbility : Ability {
    public override Type ability_type { get { return Type.Passive; } }

    public override PassiveAbility passive { get { return this; } }

    public override bool is_available { get { return character != null; } }

    [SerializeField] BuffDefinition passive_buff;

    IBuff current;

    public override void SetCharacter(Character character) {
        if (current != null) {
            current.Remove();
            current = null;
        }
        base.SetCharacter(character);
        if (character != null) {
            current = passive_buff.GetInstance();
            current.Apply(character);
        }
    }
}
