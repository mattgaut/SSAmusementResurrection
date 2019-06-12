using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBotHandler : AerialEnemyHandler {

    [SerializeField] FireBotAbilitySet abilities;
    [SerializeField] float projectile_max_range, projectile_min_range;
    [SerializeField] float field_max_range;
    [SerializeField] float min_hover, max_hover, hover_speed;
    [SerializeField] float close_distance;

    [SerializeField] Transform targeting_transform;
    [SerializeField] ParticleSystem field_particles;

    float hover_direction;
    float hover_height;

    public bool can_use_projectile {
        get {
            if (!abilities.projectile.is_available) return false;
            float distance = Vector2.Distance(target.stats.center_mass.position, targeting_transform.position);
            return distance > projectile_min_range && distance < projectile_max_range;
        }
    }

    public bool can_use_field {
        get {
            if (!abilities.field.is_available) return false;
            float distance = Vector2.Distance(target.stats.head.position, targeting_transform.position);
            return distance < field_max_range;
        }
    }

    IEnumerator Approach() {
        enemy.animator.SetBool("Mad", true);
        Vector3 target_position = target.stats.head.position;
        target_position.y += hover_height;
        input = (target_position - transform.position).normalized;
        yield return new WaitForFixedUpdate();
    }

    IEnumerator FireProjectile() {
        input = Vector2.zero;
        abilities.projectile.TryUse();
        while (abilities.projectile.is_using_ability) {
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ActivateField() {
        input = Vector2.zero;
        abilities.field.TryUse();
        if (abilities.field.is_using_ability) {
            field_particles.Play();

            yield return GameManager.instance.TeamWaitForSeconds(enemy.team, 0.25f);

            enemy.Dash(targeting_transform.rotation * Vector2.down * 3, 1f);

            while (abilities.field.is_using_ability) {
                yield return new WaitForFixedUpdate();
            }

            field_particles.Stop();
        }
    }


    protected override void Ini() {
        base.Ini();
        abilities.SetCharacter(enemy);
        hover_height = Random.Range(min_hover, max_hover);
        hover_direction = Mathf.Sign(Random.Range(0, 2) - 0.5f);
    }

    protected override void Update() {
        base.Update();
        targeting_transform.rotation = Quaternion.FromToRotation(Vector2.up, target.stats.center_mass.position - targeting_transform.position);
        hover_height += hover_direction * hover_speed * GameManager.GetDeltaTime(enemy.team);
        if (hover_height > max_hover) {
            hover_direction = -1;
        } else if (hover_height < min_hover) {
            hover_direction = 1;
        }
    }

    protected void FixedUpdate() {
        targeting_transform.rotation = Quaternion.FromToRotation(Vector2.up, target.stats.center_mass.position - targeting_transform.position);
    }
}
