using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAbility : ActiveAbility {

    [SerializeField] SingleHitAttack attack;
    [SerializeField] float damage_multiplier;
    [SerializeField] Vector2 knockback;

    [SerializeField] float dash_length;

    [SerializeField] AnimationCurve x_position;
    [SerializeField] AnimationCurve y_position;

    float dash_time;

    protected void Awake() {
        NormalizeAnimationCurves();

        attack.SetOnHit(AttackOnHit);
    }

    protected override void UseAbility(float input) {
        bool go_left = input < 0;
        Character.CustomDash dash = new Character.CustomDash(GetNextDashForce, dash_length, new Vector2(go_left ? -1 : 1, 1));
        character.Dash(dash);
        StartCoroutine(Sweep());
    }

    protected IEnumerator Sweep() {
        using_ability = true;

        attack.Enable();
        while (character.is_dashing) {
            yield return null;
        }
        attack.Disable();

        using_ability = false;
    }

    protected Vector2 GetNextDashForce(float time_at_last_step, float time) {
        return DashPositionFormula(time / dash_length) - DashPositionFormula(time_at_last_step / dash_length);
    }

    Vector2 DashPositionFormula(float percent_complete) {
        return new Vector2(x_position.Evaluate(percent_complete), y_position.Evaluate(percent_complete));
    }

    void NormalizeAnimationCurves() {
        float offset = x_position.Evaluate(0);
        for (int i = 0; i < x_position.keys.Length; i++) {
            x_position.keys[i].value -= offset;
        }

        offset = y_position.Evaluate(0);
        for (int i = 0; i < y_position.keys.Length; i++) {
            y_position.keys[i].value -= offset;
        }
    }

    void AttackOnHit(IDamageable d, Attack hit_by) {
        character.DealDamage(character.char_definition.power * damage_multiplier, d);
        Vector2 knockback = this.knockback;
        if (hit_by.transform.position.x > d.gameObject.transform.position.x) {
            knockback.x *= -1;
        }
        character.GiveKnockback(d, knockback);
    }
}
