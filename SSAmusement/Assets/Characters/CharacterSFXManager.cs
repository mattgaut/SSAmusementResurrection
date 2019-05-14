using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSFXManager : MonoBehaviour {

    [SerializeField] SFXInfo take_damage_sfx = new SFXInfo(SoundBankCodenames.sfx_character_take_damage);
    [SerializeField] SFXInfo die_sfx = new SFXInfo(SoundBankCodenames.sfx_character_die);
    [SerializeField] SFXInfo jump_sfx = new SFXInfo("sfx_character_jump");
    [SerializeField] SFXInfo land_sfx = new SFXInfo("sfx_character_land");

    private void Awake() {
        Character character = GetComponent<Character>();
        if (character) {
            character.on_take_damage += (a, b, c, d) => SoundManager.instance.LocalPlaySfx(take_damage_sfx);
            character.on_death += (a, b) => SoundManager.instance.LocalPlaySfx(die_sfx);
        }

        IInputHandler input_handler = GetComponent<IInputHandler>();
        if (input_handler != null) {
            input_handler.on_jump += (a) => SoundManager.instance.LocalPlaySfx(jump_sfx);
        }
    }
}
