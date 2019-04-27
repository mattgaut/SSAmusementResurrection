using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanCounterAbility : ActiveAbility {

    [SerializeField] Attack counter_attack;
    [SerializeField] Collider2D swordsman_hitbox;
    [SerializeField] CounterHitbox counter_hitbox;
    [SerializeField] float counter_length, min_counter_length, counter_fail_length, active_hitbox_time;
    [SerializeField] string counter_anim_trigger_name, success_anim_trigger_name, fail_anim_bool_name;


    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        counter_attack.SetSource(character);
        counter_attack.SetOnHit(AttackOnHit);
    }

    protected override void UseAbility(float input) {
        StartCoroutine(Counter());
    }

    IEnumerator Counter() {
        using_ability = true;
        character.LockMovement();
        character.LockGravity();
        character.RaiseCancelVelocityFlag();
        character.animator.SetTrigger(counter_anim_trigger_name);
        float time = 0;

        swordsman_hitbox.enabled = false;
        counter_hitbox.enabled = true;
        while (time < counter_length) {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;         
            if (counter_hitbox.was_hit && time > min_counter_length) {
                break;
            }
        }        

        time = 0;

        if (counter_hitbox.was_hit) {
            counter_hitbox.enabled = false;
            counter_attack.Enable();
            character.animator.SetTrigger(success_anim_trigger_name);

            while (time < active_hitbox_time) {
                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }

            counter_attack.Disable();
            swordsman_hitbox.enabled = true;
            character.UnlockGravity();
        } else {
            character.UnlockGravity();
            counter_hitbox.enabled = false;

            character.animator.SetBool(fail_anim_bool_name, true);
            swordsman_hitbox.enabled = true;
            while (time < counter_fail_length) {
                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            character.animator.SetBool(fail_anim_bool_name, false);
        }

        using_ability = false;
        character.UnlockMovement();
    }

    void AttackOnHit(IDamageable d, Attack hit_by) {
        character.DealDamage(character.power, d);
        d.TakeKnockback(character, new Vector3(5 * Mathf.Sign(d.gameObject.transform.position.x - transform.position.x), 5, 0));
    }
}
