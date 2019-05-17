using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainDebrisAAEffect : ActiveAbilityEffect {
    [SerializeField] Transform leftmost_spawn, rightmost_spawn;
    [SerializeField] float damage_multiplier;
    [SerializeField] float rain_duration, rain_delay, throw_length, time_between_drops;

    [SerializeField] List<Projectile> to_drop;

    [SerializeField] string anim_bool_throw;

    RNG rng;

    float time_since_last_drop;

    private void Awake() {
        rng = new RNG();
    }

    protected override void UseAbility(float input) {
        StartCoroutine(Throw());
    }

    IEnumerator Throw() {
        is_using_ability = true;
        StartCoroutine(Rain());

        character.animator.SetBool(anim_bool_throw, true);

        yield return new WaitForSeconds(rain_duration);

        character.animator.SetBool(anim_bool_throw, false);

        is_using_ability = false;
    }

    IEnumerator Rain() {
        float timer = rain_delay;
        while (timer > 0) {
            yield return new WaitForFixedUpdate();
            timer -= Time.fixedDeltaTime;
        }

        timer = rain_duration;
        time_since_last_drop = time_between_drops;
        while (timer > 0) {
            if (time_since_last_drop > time_between_drops) SpawnDebris();
            yield return new WaitForFixedUpdate();
            timer -= Time.fixedDeltaTime;
            time_since_last_drop += Time.fixedDeltaTime;
        }
    }

    void SpawnDebris() {
        time_since_last_drop -= time_between_drops;

        Projectile new_projectile = Instantiate(to_drop.GetRandom(rng));
        new_projectile.transform.position = leftmost_spawn.position;
        new_projectile.transform.position += (rightmost_spawn.position - leftmost_spawn.position) * rng.GetFloat();
        new_projectile.SetSource(character);

        new_projectile.SetOnHit(OnHit);
    }

    void OnHit(IDamageable hit, Attack hit_by) {
        hit_by.source.DealDamage(hit_by.source.power * damage_multiplier, hit, false);
    }
}
