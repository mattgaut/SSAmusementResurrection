using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneAAEffect : ActiveAbilityEffect {
    public bool is_winding_up {
        get; private set;
    }

    [SerializeField] Attack attack;
    [SerializeField] float damage_multiplier;
    [SerializeField] Vector2 knockback;
    [SerializeField] StatBuff speed_buff;
    [SerializeField] float duration;

    [SerializeField] string anim_trigger_windup, anim_trigger_end_spin;

    public void FinishWindUp() {
        is_winding_up = false;
    }

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(Cyclone());
    }

    private void Awake() {
        attack.SetOnHit(OnHit);
    }

    IEnumerator Cyclone() {
        character.animator.SetTrigger(anim_trigger_windup);

        IBuff buff = null;

        if (speed_buff) {
            buff = speed_buff.GetInstance();
        }
        
        is_using_ability = true;
        is_winding_up = true;
        // Wait for anim to finish windup
        while (is_winding_up) {
            yield return null;
        }

        buff?.Apply(character);
        attack.Enable();

        float timer = duration;
        while (timer > 0) {
            yield return new WaitForFixedUpdate();
            timer -= GameManager.GetDeltaTime(character.team);
        }

        attack.Disable();
        buff?.Remove();

        character.animator.SetTrigger(anim_trigger_end_spin);
        is_using_ability = false;
    }

    void OnHit(Character hit, Attack hit_by) {
        character.DealDamage(character.power * damage_multiplier, hit, true);
        character.GiveKnockback(hit, new Vector2(knockback.x * Mathf.Sign(hit.gameObject.transform.position.x - hit_by.transform.position.x), knockback.y));
    }
}
