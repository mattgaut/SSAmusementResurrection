using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanCounterAAEffect : ActiveAbilityEffect {

    [SerializeField] Attack counter_attack;
    [SerializeField] Collider2D swordsman_hitbox;
    [SerializeField] CounterHitbox counter_hitbox;
    [SerializeField] float counter_length, min_counter_length, counter_fail_length, active_hitbox_time;
    [SerializeField] AnimParameterEvent counter_anim, success_anim, fail_anim;
    [SerializeField] Vector2 knockback;


    public override void SetCharacter(Character character) {
        base.SetCharacter(character);

        counter_attack.SetSource(character);
        counter_attack.SetOnHit(AttackOnHit);
    }

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(Counter());
    }

    IEnumerator Counter() {
        is_using_ability = true;
        int move_lock = character.LockMovement();
        int grav_lock = character.LockGravity();
        character.RaiseCancelVelocityFlag();
        character.animator.ProccessAnimParameterEvent(counter_anim);
        float time = 0;

        swordsman_hitbox.enabled = false;
        counter_hitbox.enabled = true;
        while (time < counter_length) {
            yield return new WaitForFixedUpdate();
            time += GameManager.GetFixedDeltaTime(character.team);         
            if (counter_hitbox.was_hit && time > min_counter_length) {
                break;
            }
        }        

        time = 0;

        if (counter_hitbox.was_hit) {
            counter_hitbox.enabled = false;
            counter_attack.Enable();
            character.animator.ProccessAnimParameterEvent(success_anim);

            while (time < active_hitbox_time) {
                yield return new WaitForFixedUpdate();
                time += GameManager.GetFixedDeltaTime(character.team);
            }

            counter_attack.Disable();
            swordsman_hitbox.enabled = true;
            character.UnlockGravity(grav_lock);
        } else {
            character.UnlockGravity(grav_lock);
            counter_hitbox.enabled = false;

            character.animator.ProccessAnimParameterEvent(fail_anim);
            swordsman_hitbox.enabled = true;
            while (time < counter_fail_length) {
                yield return new WaitForFixedUpdate();
                time += GameManager.GetFixedDeltaTime(character.team);
            }
            character.animator.ProccessAnimParameterEvent(fail_anim.Reverse());
        }

        is_using_ability = false;
        character.UnlockMovement(move_lock);
    }

    void AttackOnHit(Character d, Attack hit_by) {
        character.DealDamage(character.power, d);
        character.GiveKnockback(d, new Vector2(knockback.x * Mathf.Sign(d.gameObject.transform.position.x - transform.position.x), knockback.y));
    }
}
