﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Swordsman))]
public class SwordsmanInputHandler : PlayerInputHandler {

    Swordsman swordsman;

    MeleeAttackAbility basic_ability;
    SwordsmanCounterAbility ability_1;

    protected override void OnAwake() {
        swordsman = GetComponent<Swordsman>();

        basic_ability = swordsman.basic_ability as MeleeAttackAbility;
        ability_1 = swordsman.ability_1 as SwordsmanCounterAbility;
    }

    protected override void OnBasicAttackButton() {
        basic_ability.TryUse();
    }

    protected override void OnSkill1Button() {
        ability_1.TryUse();
    }

}
