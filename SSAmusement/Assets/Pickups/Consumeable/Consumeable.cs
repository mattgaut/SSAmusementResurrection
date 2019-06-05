using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Consumeable : Pickup {

    public ActiveAbility ability { get { return _ability; } }

    [SerializeField] ActiveChargeAbility _ability;

    protected override void OnPickup(Player p) {
        PickupEffect(p);

        SoundManager.instance?.LocalPlaySfx(on_pickup_audio_clip);
    }

    protected override void PickupEffect(Player player) {
        player.inventory.AddConsumeable(this);
    }
}
