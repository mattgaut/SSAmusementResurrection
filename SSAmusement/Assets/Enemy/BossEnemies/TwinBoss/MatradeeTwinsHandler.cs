using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatradeeTwinsHandler : GroundedEnemyHandler {

    [SerializeField] MatradeeTwinsAbilitySet abilities;

    [SerializeField] float spin_target_range;
    [SerializeField] float spin_duration;

    protected override void Start() {
        base.Start();
        abilities.SetCharacter(enemy);
    }

    IEnumerator Spin() {
        float difference = target.transform.position.x - transform.position.x;
        while (Mathf.Abs(difference) > spin_target_range) {
            _input.x = Mathf.Sign(difference);
            yield return new WaitForFixedUpdate();
            difference = target.transform.position.x - transform.position.x;
        }
        _input.x = 0;

        if (abilities.cyclone.TryUse()) {
            // Follow Target
            float timer = spin_duration;

            while (abilities.cyclone.winding_up) {
                yield return null;
            }

            while (abilities.cyclone.using_ability) {
                if (Mathf.Abs(difference) > 0.2f) {
                    _input.x = Mathf.Sign(difference)/2f;
                } else {
                    _input.x = 0;
                }
                yield return new WaitForFixedUpdate();
                timer -= Time.deltaTime;
                difference = target.transform.position.x - transform.position.x;
            }
            _input.x = 0;
        }
    }
}
