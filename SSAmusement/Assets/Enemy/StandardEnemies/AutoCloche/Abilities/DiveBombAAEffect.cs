using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveBombAAEffect : ActiveAbilityEffect {

    [SerializeField] SingleHitAttack attack;
    [SerializeField] float damage_multiplier;
    [SerializeField] Vector2 knockback;

    [SerializeField] float distance;
    [SerializeField] float speed;

    public override void SetCharacter(Character character) {
        base.SetCharacter(character);
        attack.SetOnHit(AttackOnHit);
    }

    protected override void UseAbility(float input) {
        character.Dash(Vector2.down * distance, distance / speed);
        StartCoroutine(DiveBomb());
    }

    protected IEnumerator DiveBomb() {
        is_using_ability = true;

        attack.Enable();
        while (character.is_dashing) {
            yield return null;
        }
        attack.Disable();

        is_using_ability = false;
    }

    
    void AttackOnHit(IDamageable d, Attack hit_by) {
        character.DealDamage(character.char_definition.power * damage_multiplier, d);
        character.GiveKnockback(d, knockback);
    }
}
