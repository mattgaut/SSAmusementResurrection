using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverOverTime : MonoBehaviour {

    [SerializeField] float max_recovery_percent, max_recovery_amount;
    [SerializeField] float health_per_second;

    Player recover_target;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            recover_target = collision.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (collision.GetComponentInParent<Player>() == recover_target) {
                recover_target = null;
            }
        }
    }

    private void Update() {
        if (recover_target && recover_target.health.percent <= max_recovery_percent && !recover_target.health.is_max) {
            float recover_amount = health_per_second * Time.deltaTime;
            if (recover_amount > max_recovery_amount) {
                recover_amount = max_recovery_amount;
            }
            recover_target.RestoreHealth(recover_amount);
            max_recovery_amount -= recover_amount;
        }
    }
}
