using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterTimeAAEffect : ActiveAbilityEffect {
    [SerializeField] float time_scale_factor;
    [SerializeField] Character.Team affected_team;

    [SerializeField] float unscaled_duration;

    protected override void UseAbilityEffect(float input) {
        StartCoroutine(TimerCoroutine(time_scale_factor));
    }

    IEnumerator TimerCoroutine(float new_time_scale) {
        GameManager.instance.AddTeamTimeScaleModifier(affected_team, time_scale_factor);

        yield return new WaitForSeconds(unscaled_duration);

        GameManager.instance.RemoveTeamTimeScaleModifier(affected_team, time_scale_factor);
    }
}
