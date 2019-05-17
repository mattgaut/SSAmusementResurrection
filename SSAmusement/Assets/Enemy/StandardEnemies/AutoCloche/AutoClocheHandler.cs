using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoClocheHandler : AerialEnemyHandler {

    [SerializeField] AutoClocheAbilitySet abilities;
    [SerializeField] float chase_time_out;

    public bool can_dive_bomb { get { return abilities.dive_bomb.is_available && IsNextAbility(abilities.dive_bomb); } }

    public bool can_sweep { get { return abilities.sweep.is_available && IsNextAbility(abilities.sweep); } }

    Ability next_ability = null;

    protected override void Start() {
        base.Start();
        abilities.SetCharacter(enemy);
        next_ability = abilities.GetAbility(Random.Range(0, abilities.count));
    }

     protected IEnumerator DiveBomb() {
        // Hover over enemy
        Vector3 target_position = target.char_definition.center_mass.position + Vector3.up * 2f;

        float timer = 0;
        bool timed_out = false;
        while (transform.position.y - target_position.y < -0.25f || transform.position.y - target_position.y > 2f  || Mathf.Abs(transform.position.x - target_position.x) > 0.25f) {
            _input = (target_position - transform.position).normalized;
            yield return new WaitForFixedUpdate();
            target_position = target.char_definition.center_mass.position + Vector3.up * 2f;
            if (timer > chase_time_out) {
                timed_out = true;
                break;
            }
        }
        if (!timed_out) {
            velocity /= 3f;
            _input = Vector2.zero;

            auto_tilt = false;
            // Tilt Upside Down
            float tilt_time = .5f;
            float start_tilt_time = tilt_time;
            float start_tilt = pivot_object.transform.rotation.eulerAngles.z;
            if (start_tilt < 0) start_tilt += 360;
            while (tilt_time > 0) {
                yield return new WaitForFixedUpdate();
                tilt_time -= Time.deltaTime;
                LerpToUnclampedTilt(start_tilt, 180, (start_tilt_time - tilt_time) / start_tilt_time);
            }
            SetUnclampedTilt(180);

            if (abilities.dive_bomb.TryUse()) {
                while (abilities.dive_bomb.is_using_ability) {
                    yield return null;
                }
                next_ability = null;
            }

            auto_tilt = true;
        }
    }

    protected IEnumerator Sweep() {
        // Hover towards enemy
        Vector3 offset;
        if (transform.position.x < target.transform.position.x) {
            offset = Vector3.left * 3f;
        } else {
            offset = Vector3.right * 3f;
        }
        Vector3 target_position = target.char_definition.head.position + offset;

        float timer = 0;
        bool timed_out = false;
        while (transform.position.x - target_position.x < -1f || transform.position.x - target_position.x > 0.5f || Mathf.Abs(transform.position.y - target_position.y) > 0.2f) {
            _input = (target_position - transform.position).normalized;
            yield return new WaitForFixedUpdate();
            target_position = target.char_definition.head.position + offset;
            timer += Time.fixedDeltaTime;
            if (timer > chase_time_out) {
                timed_out = true;
                break;
            }
        }

        if (!timed_out && abilities.sweep.TryUse(target.transform.position.x - transform.position.x)) {
            while (abilities.sweep.is_using_ability) {
                yield return null;
            }
            next_ability = null;
        }
    }

    bool IsNextAbility(Ability ability) {
        return next_ability == null || next_ability.ability_name == ability.ability_name;
    }
}
