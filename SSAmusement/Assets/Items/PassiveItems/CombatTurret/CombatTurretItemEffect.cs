using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CombatTurretItemEffect : OnHitItemEffect {

    [SerializeField] float energy_per_shot;
    [SerializeField] float multiplier;
    [SerializeField] float aggro_range;
    [SerializeField] float percent_charge_per_second;

    [SerializeField] PetTurretController pet;

    float stored_energy;
    CircleCollider2D aggro_range_collider;

    List<Character> targets;

    Character owner;

    protected override void OnPickup() {
        base.OnPickup();

        pet.SetOwner(item.owner);
        pet.transform.SetParent(null, true);
        pet.transform.localPosition = item.owner.transform.position;
        pet.gameObject.SetActive(true);
        item.owner.inventory.AddPet();
        pet.GetComponent<SpriteRenderer>().sortingOrder = item.owner.inventory.PetCount() * -1;
        pet.SetOrbit(item.owner.char_definition.center_mass, item.owner.inventory.PetCount() / 10f);

        transform.localPosition = Vector3.zero;

        owner = item.owner;

        targets = new List<Character>();
    }

    protected override void OnDrop() {
        base.OnDrop();

        item.owner.inventory.RemovePet();

        owner = null;

        pet.transform.SetParent(transform, true);
        pet.gameObject.SetActive(false);
    }


    protected override void OnOwnerHitEnemy(Character character, float pre_damage, float post_damage, Character hit) {
        stored_energy += post_damage;
        if (stored_energy >= energy_per_shot) {
            Shoot();
        }
    }

    protected Character GetRandomTarget() {
        Character to_return = null;
        targets.Shuffle();
        
        for (int i = targets.Count - 1; i >= 0; i--) {
            to_return = targets[i];
            if (to_return != null && to_return.alive) {
                break;
            } else {
                targets.RemoveAt(i);
                to_return = null;
            }
        }
        return to_return;
    }

    protected void OnTurretLaserHit(Character hitter, Character hit, float damage) {
        hitter.DealDamage(damage * multiplier, hit, false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & 1 << LayerMask.NameToLayer("Enemy")) != 0) {
            targets.Add(collision.gameObject.GetComponentInParent<Character>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if ((1 << collision.gameObject.layer & LayerMask.GetMask("Enemy")) != 0) {
            targets.Remove(collision.gameObject.GetComponentInParent<Character>());
        }
    }

    private void FixedUpdate() {
        if (stored_energy < energy_per_shot) {
            stored_energy += (percent_charge_per_second * energy_per_shot) * GameManager.GetFixedDeltaTime(owner.team);
            if (stored_energy > energy_per_shot) {
                stored_energy = energy_per_shot;
            }
        }
        if (stored_energy >= energy_per_shot) {
            Shoot();
        }
    }

    private void Awake() {
        aggro_range_collider = GetComponent<CircleCollider2D>();
        aggro_range_collider.radius = aggro_range;
    }

    void Shoot() {
        Character to_shoot = GetRandomTarget();
        if (to_shoot != null) {
            float damage_to_send = stored_energy;
            pet.AddTargetToQueue(to_shoot, (actually_hit) => OnTurretLaserHit(owner, actually_hit, damage_to_send), () => stored_energy += damage_to_send);
            stored_energy = 0;
        }
    }

    private void OnDestroy() {
        if (pet.gameObject) Destroy(pet.gameObject);
    }
}
