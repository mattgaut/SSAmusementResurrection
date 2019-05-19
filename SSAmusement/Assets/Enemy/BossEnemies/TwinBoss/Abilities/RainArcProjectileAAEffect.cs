using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainArcProjectileAAEffect : ActiveAbilityEffect {

    [SerializeField] Transform spawn_point;

    [SerializeField] float rain_duration, time_between_projectiles;
    [SerializeField] float damage_multiplier;
    [SerializeField] float min_angle, max_angle;
    [SerializeField] float min_force, max_force;

    [SerializeField] string anim_bool_throw;

    [SerializeField] Projectile projectile_prefab;

    float time_since_last_projectile;

    RNG rng;

    private void Awake() {
        rng = new RNG();
    }

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(Rain());
    }

    IEnumerator Rain() {
        is_using_ability = true;        

        character.animator.SetBool(anim_bool_throw, true);

        float timer = rain_duration;
        time_since_last_projectile = time_between_projectiles;
        while (timer > 0) {
            if (time_since_last_projectile > time_between_projectiles) SpawnProjectile();
            yield return new WaitForFixedUpdate();
            timer -= Time.fixedDeltaTime;
            time_since_last_projectile += Time.fixedDeltaTime;
        }

        character.animator.SetBool(anim_bool_throw, false);

        is_using_ability = false;
    }

    void SpawnProjectile() {
        time_since_last_projectile -= time_between_projectiles;

        Projectile new_projectile = Instantiate(projectile_prefab);
        new_projectile.transform.position = spawn_point.position;

        new_projectile.SetSpeedAndDirection(Quaternion.Euler(0,0, rng.GetFloat(min_angle, max_angle)) * Vector2.up * rng.GetFloat(min_force, max_force));

        new_projectile.SetOnHit(OnHit);
    }

    void OnHit(Character hit, Attack hit_by) {
        character.DealDamage(character.power * damage_multiplier, hit, false);
    }


}
