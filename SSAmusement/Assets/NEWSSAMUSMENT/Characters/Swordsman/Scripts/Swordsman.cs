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
    public override Ability ability_2 { get { return dash; } }

    [SerializeField] MeleeAttackAbility _basic_ability;
    [SerializeField] SwordsmanCounterAbility counter;
    [SerializeField] DashAbility dash;

    protected override void OnAwake() {
        base.OnAwake();
        _basic_ability.SetCanUse(() => can_use_basic_ability);
        counter.SetCanUse(() => can_use_skill_1);
        dash.SetCanUse(() => can_use_skill_2);
    }

    protected override void OnStart() {
        base.OnStart();

        player_display.SetAbilityDisplay(_basic_ability, 0);
        player_display.SetAbilityDisplay(counter, 1);
        player_display.SetAbilityDisplay(dash, 2);
    }
}