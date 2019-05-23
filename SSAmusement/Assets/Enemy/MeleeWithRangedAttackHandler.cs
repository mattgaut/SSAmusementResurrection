using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWithRangedAttackHandler : MeleeAttackHandler {

    [SerializeField] float time_between_ranged_attacks;
    [SerializeField] Projectile projectile;
    [SerializeField] float min_projectile_attack_range, max_projectile_attack_range;
    [SerializeField] Vector3 projectile_attack_knockback;

    [SerializeField] Transform projectile_spawn_transform;

    float last_projectile_attack;
    bool ranged_attack_over;

    public bool IsInProjectileAttackInRange() {
        float distance = Mathf.Abs(target.transform.position.x - transform.position.x);
        return distance < max_projectile_attack_range && distance > min_projectile_attack_range;
    }

    public bool IsProjectileAttackReady() {
        return time_between_ranged_attacks < last_projectile_attack;
    }

    public bool CanRangedAttack() {
        return IsProjectileAttackReady() && IsInProjectileAttackInRange(); 
    }

    protected override void Ini() {
        base.Ini();
        last_projectile_attack = time_between_ranged_attacks;
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
        new_projectile.transform.position = projectile_spawn_transform.position;
        new_projectile.transform.rotation = projectile_spawn_transform.rotation;

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
