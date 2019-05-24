using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWithRangedAttackHandler : MeleeAttackHandler {

    [SerializeField] float time_between_ranged_attacks;
    [SerializeField] Projectile projectile;
    [SerializeField] float min_projectile_attack_range, max_projectile_attack_range;
    [SerializeField] Vector3 projectile_attack_knockback;
    [SerializeField] float ranged_follow_distance;

    [SerializeField] Transform projectile_spawn_transform;

    float last_projectile_attack;
    bool ranged_attack_over;

    Vector2 last_good_target;

    public bool IsInProjectileAttackInRange() {
        Vector2 distance = target.char_definition.center_mass.position - projectile_spawn_transform.position;
        distance.x = Mathf.Abs(distance.x);
        if (distance.x > max_projectile_attack_range || distance.x < min_projectile_attack_range) {
            return false;
        }

        if (projectile.CanLaunchTowardsTarget(distance)) {
            last_good_target = distance;
            return true;
        } else {
            return false;
        }

    }

    public bool IsTooCloseRanged() {
        float distance =  Mathf.Abs(target.char_definition.center_mass.position.x - transform.position.x);
        return distance < ranged_follow_distance;
    }

    public bool IsProjectileAttackReady() {
        return time_between_ranged_attacks < last_projectile_attack;
    }

    public bool CanRangedAttack() {
        return IsProjectileAttackReady() && IsInProjectileAttackInRange(); 
    }

    protected override void Ini() {
        base.Ini();
        last_projectile_attack = time_between_ranged_attacks/2f;
    }

    protected IEnumerator RangedAttack() {
        Face(target.transform.position.x - transform.position.x);
        can_flip = false;
        last_projectile_attack = 0;
        enemy.animator.SetTrigger("RangedAttack");

        _input.x = 0;

        ranged_attack_over = false;
        while (!ranged_attack_over) {
            yield return new WaitForFixedUpdate();
        }
        yield return null;
        can_flip = true;
    }

    public void AnimRangedAttackOver() {
        ranged_attack_over = true;
    }
    public void AnimRangedAttackRelease() {
        Projectile new_projectile = Instantiate(projectile);

        Vector2 distance = target.char_definition.center_mass.position - projectile_spawn_transform.position;
        distance.x = Mathf.Abs(distance.x);

        if (!new_projectile.LaunchTowardsTarget(distance)) {
            new_projectile.LaunchTowardsTarget(last_good_target);
        }

        new_projectile.transform.position = projectile_spawn_transform.position;
        new_projectile.transform.rotation = projectile_spawn_transform.rotation;

        new_projectile.Flip();

        new_projectile.SetOnHit(RangedAttackOnHit);
        new_projectile.SetSource(enemy);
    }

    protected override void Update() {
        base.Update();
        last_projectile_attack += GameManager.GetDeltaTime(enemy.team);
    }

    void RangedAttackOnHit(Character c, Attack a) {
        enemy.DealDamage(enemy.power, c);
        if (projectile_attack_knockback != Vector3.zero) {
            Vector3 real_knockback = projectile_attack_knockback;
            if (c.gameObject.transform.position.x < a.transform.position.x) {
                real_knockback.x *= -1;
            }
            enemy.GiveKnockback(c, real_knockback);
        }
    }
}
