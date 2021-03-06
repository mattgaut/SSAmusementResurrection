﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAAEffect : ActiveAbilityEffect {

    [SerializeField] float wind_up_time;
    [SerializeField] float active_hitbox_time;

    [SerializeField] Attack attack;

    [SerializeField] AnimParameterEvent begin_anim;

    [SerializeField] Formula damage;
    [SerializeField] Vector2 knockback;

    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        attack.SetSource(character);
        attack.SetOnHit(AttackOnHit);
    }

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(AbilityCoroutine());
    }

    protected virtual void AttackOnHit(Character d, Attack hit_by) {
        character.DealDamage(damage.GetValue(character), d);
        character.GiveKnockback(d, new Vector2(knockback.x * Mathf.Sign(d.gameObject.transform.position.x - transform.position.x), knockback.y));
    }

    IEnumerator AbilityCoroutine() {
        character.animator.ProccessAnimParameterEvent(begin_anim);
        float time = 0;
        is_using_ability = true;

        while (time < wind_up_time) { // wait before hitbox active
            time += GameManager.GetFixedDeltaTime(character.team);
            yield return new WaitForFixedUpdate();
        }
        attack.Enable();
        time = 0;
        while (time < active_hitbox_time) { // length of basic attack
            time += GameManager.GetFixedDeltaTime(character.team);
            yield return new WaitForFixedUpdate();
        }
        attack.Disable();
        is_using_ability = false;
    }
}
