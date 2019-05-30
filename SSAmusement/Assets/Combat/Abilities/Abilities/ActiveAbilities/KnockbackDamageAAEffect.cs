using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class KnockbackDamageAAEffect : ActiveAbilityEffect {

    [SerializeField] SingleHitAttack hitbox;
    [SerializeField] float active_hitbox_duration;
    [SerializeField] Formula damage;
    [SerializeField] Vector2 knockback;

    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        hitbox.SetSource(character);
        hitbox.SetOnHit(OnHit);
    }

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(TimeHitboxRoutine());
    }
    
    IEnumerator TimeHitboxRoutine() {
        hitbox.Enable();
        yield return GameManager.instance.TeamWaitForSeconds(character.team, active_hitbox_duration);
        hitbox.Disable();
    }

    void OnHit(Character hit, Attack attack) {
        character.DealDamage(damage.GetValue(character), hit);
        Vector2 real_knockback = knockback;
        real_knockback = Quaternion.FromToRotation(knockback, hit.transform.position - character.transform.position) * knockback;
        character.GiveKnockback(hit, real_knockback);
    }
}
