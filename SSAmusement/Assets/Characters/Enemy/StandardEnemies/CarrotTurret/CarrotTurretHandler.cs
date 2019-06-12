using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotTurretHandler : GroundedEnemyHandler {

    public bool can_shoot {
        get { return abilities.shoot.is_available; }
    }

    public bool can_face_target {
        get { return can_change_facing; }
    }

    [SerializeField] bool can_change_facing;
    [SerializeField] Projectile shot_object;
    [SerializeField] Transform projectile_spawn_point;

    [SerializeField] CarrotTurretAbilitySet abilities;

    protected override void Start() {
        base.Start();
        abilities.SetCharacter(enemy);
    }

    IEnumerator FaceTarget() {
        if (can_change_facing) Face(target.transform.position.x - transform.position.x);
        yield return new WaitForFixedUpdate();
    }

    IEnumerator Shoot() {
        if (can_change_facing) Face(target.transform.position.x - transform.position.x);
        abilities.shoot.TryUse(facing);
        yield return new WaitForFixedUpdate();
    }

    IEnumerator Wait() {
        yield return new WaitForFixedUpdate();
    }

}
