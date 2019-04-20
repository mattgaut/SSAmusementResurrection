using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Player {
    public override bool can_use_basic_ability {
        get {
            return !(_basic_ability.using_ability || counter.using_ability);
        }
    }
    public override bool can_use_skill_1 {     
        get {
            return !(_basic_ability.using_ability || counter.using_ability);
        }
    }

    public override Ability basic_ability { get { return _basic_ability; } }
    public override Ability ability_1 { get { return counter; } }

    [SerializeField] MeleeAttackAbility _basic_ability;
    [SerializeField] SwordsmanCounterAbility counter;

    protected override void OnAwake() {
        base.OnAwake();
        _basic_ability.SetCanUse(() => can_use_basic_ability);
        counter.SetCanUse(() => can_use_skill_1);
    }
}