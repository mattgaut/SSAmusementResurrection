using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDefenseTurretItemEffect : OnTakeDamageItemEffect {

    [SerializeField] float damage_multiplier;

    [SerializeField] AutoDefenseTurretController controller;
    [SerializeField] Vector2 offset;

    public override void OnPickup(Item item) {
        base.OnPickup(item);
        controller.transform.SetParent(null, true);
        controller.transform.localPosition = offset;
        controller.gameObject.SetActive(true);
        controller.GetComponent<SpriteRenderer>().sortingOrder = item.owner.inventory.ItemCount(item.item_name) * -1;
        controller.SetOrbit(item.owner.char_definition.center_mass);
    }

    public override void OnDrop(Item item) {
        base.OnDrop(item);

        controller.transform.SetParent(transform, true);
        controller.gameObject.SetActive(false);
    }

    protected override void OnTakeDamage(Character hit, float pre_damage, float post_damage, ICombatant source) {
        controller.AddTargetToQueue(source.char_definition, () => hit.DealDamage(post_damage * damage_multiplier, source, false));
    }
}
