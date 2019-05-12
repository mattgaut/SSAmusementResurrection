using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackAbility : ActiveAbility {

    [SerializeField] float wind_up_time;
    [SerializeField] float active_hitbox_time;

    [SerializeField] Attack attack;

    [SerializeField] string anim_trigger_name;

    [SerializeField] Vector2 knockback;

    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        attack.SetSource(character);
        attack.SetOnHit(AttackOnHit);
    }

    protected override void UseAbility(float input) {
        StartCoroutine(AbilityCoroutine());
    }

    IEnumerator AbilityCoroutine() {
        character.animator.SetTrigger(anim_trigger_name);
        float time = 0;
        using_ability = true;

        while (time < wind_up_time) { // wait before hitbox active
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        attack.Enable();
        time = 0;
        while (time < active_hitbox_time) { // length of basic attack
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        attack.Disable();
        using_ability = false;
    }

    void AttackOnHit(IDamageable d, Attack hit_by) {
        character.DealDamage(character.power, d);
        character.GiveKnockback(d, new Vector2(knockback.x * Mathf.Sign(d.gameObject.transform.position.x - transform.position.x), knockback.y));
    }
}
