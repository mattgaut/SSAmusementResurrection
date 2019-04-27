using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetTurretController : MonoBehaviour {
    [SerializeField] HomingCallbackProjectile laser;

    [SerializeField] Animator animator;

    [SerializeField] Transform orbit_center;

    List<TargetCallback> queue;

    float base_orbit_speed = 5f;
    float orbit_speed;

    [SerializeField] float orbit_period;
    [SerializeField] Vector2 orbit_offset;
    [SerializeField] float min_orbit_angle, max_orbit_angle;

    float timer;
    int orbit_direction = 1;
    Vector2 orbit_target;

    private void Awake() {
        queue = new List<TargetCallback>();
    }

    public void SetLaser(HomingCallbackProjectile laser) {
        this.laser = laser;
    }

    public void SetOrbit(Transform transform) {
        orbit_center = transform;
    }

    /// <summary>
    /// Add target to queue attached to callbacks
    /// </summary>
    /// <param name="target"></param>
    /// <param name="hit_callback">Callback that executes on character laser hits</param>
    /// <param name="fail_callback">Callback that executes if target is no longer valid when laser fired</param>
    public void AddTargetToQueue(Character target, Action<Character> hit_callback, Action fail_callback = null) {
        queue.Add(new TargetCallback(target, hit_callback, fail_callback));
        animator.SetBool("LasersQueued", true);
    }

    /// <summary>
    /// Should only be called via Animation
    /// 
    /// Searches Queue in order for a valid target to Shoot.
    /// Invokes callback for all failed targets.
    /// </summary>
    public void ShootQueuedLaser() {
        TargetCallback next_target = null;
        while (next_target == null && queue.Count != 0) {
            next_target = queue[0];
            queue.RemoveAt(0);
            if (next_target.target == null) {
                if (next_target.fail_callback != null) next_target.fail_callback.Invoke();
                next_target = null;
            }
        }
        if (next_target != null) {
            SpawnLaser(next_target);    
        }
        animator.SetBool("LasersQueued", queue.Count > 0);
    }

    void SpawnLaser(TargetCallback target_callback) {
        HomingCallbackProjectile new_laser = Instantiate(laser, transform.position, Quaternion.identity);
        laser.transform.position = transform.position;

        Transform target = target_callback.target.char_definition.center_mass;
        new_laser.SetTarget(target, target_callback.success_callback);

        float angle = 0;
        Vector3 to_target = target.transform.position - transform.position;
        if (to_target.x < 0) {
            angle = Vector2.SignedAngle(Vector2.left, to_target) + 180;
        } else {
            angle = Vector2.SignedAngle(Vector2.right, to_target);
        }

        new_laser.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void LateUpdate() {
        timer += Time.deltaTime;
        if (timer > orbit_period) {
            timer -= orbit_period;
            orbit_direction *= -1;
        }

        float angle; 

        if (orbit_direction > 0) {
            angle = Mathf.Lerp(min_orbit_angle, max_orbit_angle, timer / orbit_period);
        } else {
            angle = Mathf.Lerp(max_orbit_angle, min_orbit_angle, timer / orbit_period);
        }

        orbit_target = (orbit_center.position + (Quaternion.Euler(0, 0, angle) * orbit_offset)); 

        orbit_speed = base_orbit_speed * (1 + (Vector2.Distance(orbit_target, transform.position) / base_orbit_speed));

        transform.position += ((Vector3)orbit_target - transform.position) * orbit_speed * Time.deltaTime;
    }

    class TargetCallback {
        public Character target;
        public Action<Character> success_callback;
        public Action fail_callback;

        public TargetCallback(Character target, Action<Character> success_callback, Action fail_callback) {
            this.target = target;
            this.success_callback = success_callback;
            this.fail_callback = fail_callback;
        }
    }
}
