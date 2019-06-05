using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Consumeable : Pickup {

    protected override void OnPickup(Player p) {
        PickupEffect(p);

        SoundManager.instance?.LocalPlaySfx(on_pickup_audio_clip);
    }

    protected override void PickupEffect(Player player) {
        player.inventory.AddConsumeable(this);
    }
}
