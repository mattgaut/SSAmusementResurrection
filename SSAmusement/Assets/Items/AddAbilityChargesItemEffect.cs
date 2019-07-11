using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAbilityChargesItemEffect : ItemEffect {
    [SerializeField] int charges;
    [SerializeField] List<int> abilities;

    protected override void OnDrop() {
        foreach (int i in abilities) {
            item.owner.abilities.GetAbility(i)?.active_cooldown?.ModifyBonusCharges(-charges);
        }
    }

    protected override void OnPickup() {
        foreach (int i in abilities) {
            item.owner.abilities.GetAbility(i)?.active_cooldown?.ModifyBonusCharges(charges);
        }
    }
}
