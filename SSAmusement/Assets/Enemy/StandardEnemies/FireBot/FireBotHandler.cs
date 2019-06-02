using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBotHandler : AerialEnemyHandler {

    [SerializeField] FireBotAbilitySet abilities;
    [SerializeField] float projectile_max_range, projectile_min_range;
    [SerializeField] float field_max_range;
    [SerializeField] float min_hover, max_hover, hover_speed;
    [SerializeField] float close_distance;

    [SerializeField] Transform projectile_targeting_transform;

    float hover_direction;
    float hover_height;

    public bool can_use_projectile {
        get {
            if (!abilities.projectile.is_available) return false;
            float distance = Vector2.Distance(target.char_definition.center_mass.position, projectile_targeting_transform.position);
            return distance > projectile_min_range && distance < projectile_max_range;
        }
    }

    public bool can_use_field {
        get {
            if (!abilities.field.is_available) return false;
            float distance = Mathf.Abs(target.transform.position.x - transform.position.x);
            return distance < projectile_max_range;
        }
    }

    IEnumerator Approach() {
        enemy.animator.SetBool("Mad", true);
        Vector3 target_position = target.char_definition.head.position;
        target_position.y += hover_height;
        input = (target_position - transform.position).normalized;
        yield return new WaitForFixedUpdate();
    }

    IEnumerator FireProjectile() {
        abilities.projectile.TryUse();
        while (abilities.projectile.is_using_ability) {
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ActivateField() {
        abilities.field.TryUse();
        while (abilities.field.is_using_ability) {
            yield return new WaitForFixedUpdate();
        }
    }


    protected override void Ini() {
        base.Ini();
        abilities.SetCharacter(enemy.character);
        hover_height = Random.Range(min_hover, max_hover);
        hover_direction = Mathf.Sign(Random.Range(0, 2) - 0.5f);
    }

    protected override void Update() {
        base.Update();
        hover_height += hover_direction * hover_speed * GameManager.GetDeltaTime(enemy.team);
        if (hover_height > max_hover) {
            hover_direction = -1;
        } else if (hover_height < min_hover) {
            hover_direction = 1;
        }
    }

    protected void FixedUpdate() {
        projectile_targeting_transform.rotation = Quaternion.FromToRotation(Vector2.up, target.char_definition.center_mass.position - projectile_targeting_transform.position);
    }
}
