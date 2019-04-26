using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDefenseTurretItemEffect : OnTakeDamageItemEffect {

    [SerializeField] float damage_multiplier;

    [SerializeField] PetTurretController pet;

    public override void OnPickup(Item item) {
        base.OnPickup(item);
        pet.transform.SetParent(null, true);
        pet.transform.localPosition = item.owner.transform.position;
        pet.gameObject.SetActive(true);
        item.owner.inventory.AddPet();
        pet.GetComponent<SpriteRenderer>().sortingOrder = -1 * item.owner.inventory.PetCount();
        pet.SetOrbit(item.owner.char_definition.center_mass);
    }

    public override void OnDrop(Item item) {
        base.OnDrop(item);

        item.owner.inventory.RemovePet();

        pet.transform.SetParent(transform, true);
        pet.gameObject.SetActive(false);
    }

    protected override void OnTakeDamage(Character hit, float pre_damage, float post_damage, ICombatant source) {
        pet.AddTargetToQueue(source.char_definition, () => hit.DealDamage(post_damage * damage_multiplier, source, false));
    }
}
