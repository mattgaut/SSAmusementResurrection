using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDefenseTurretItemEffect : OnTakeDamageItemEffect {

    [SerializeField] float damage_multiplier;

    [SerializeField] PetTurretController pet;

    protected override void OnInitialPickup() {
        base.OnInitialPickup();
        pet.SetOwner(item.owner);
        pet.transform.SetParent(null, true);
        pet.transform.localPosition = item.owner.transform.position;
        pet.gameObject.SetActive(true);
        item.owner.inventory.AddPet();
        pet.GetComponent<SpriteRenderer>().sortingOrder = -1 * item.owner.inventory.PetCount();
        pet.SetOrbit(item.owner.stats.center_mass, item.owner.inventory.PetCount() / 10f);
    }

    protected override void OnFinalDrop() {
        base.OnFinalDrop();

        item.owner.inventory.RemovePet();

        pet.transform.SetParent(transform, true);
        pet.gameObject.SetActive(false);
    }

    protected override void OnTakeDamage(Character hit, float pre_damage, float post_damage, Character source) {
        pet.AddTargetToQueue(source, (target) => hit.DealDamage(post_damage * damage_multiplier * item.stack_count, target, true));
    }

    private void OnDestroy() {
        if (pet != null) Destroy(pet.gameObject);
    }
}
