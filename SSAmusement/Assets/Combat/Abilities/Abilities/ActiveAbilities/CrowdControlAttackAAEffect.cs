using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdControlAttackAAEffect : ActiveAbilityEffect {

    [SerializeField] float duration;
    [SerializeField] CrowdControl.Type crowd_control_to_apply;
    [SerializeField] SingleHitAttack attack;

    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        attack.SetSource(character);
        attack.SetOnHit(OnHit);
    }

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(CCFieldRoutine(0.2f, 0.2f));
    }

    IEnumerator CCFieldRoutine(float delay, float active_hitbox_length) {
        is_using_ability = true;
        yield return GameManager.instance.TeamWaitForSeconds(character.team, delay);
        is_using_ability = false;

        attack.Enable();
        yield return GameManager.instance.TeamWaitForSeconds(character.team, active_hitbox_length);
        attack.Disable();
    }

    void OnHit(Character hit, Attack hit_by) {
        hit.crowd_control_effects.ApplyCC(crowd_control_to_apply, duration, character);
    }
}
