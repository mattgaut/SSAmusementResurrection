﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnSpendEnergyItemEffect : ItemEffect {
    protected override void OnFinalDrop() {
        item.owner.on_spend_energy -= OnSpendEnergy;
    }

    protected override void OnInitialPickup() {
        item.owner.on_spend_energy += OnSpendEnergy;
    }

    protected abstract void OnSpendEnergy(Character source, float spent);
}