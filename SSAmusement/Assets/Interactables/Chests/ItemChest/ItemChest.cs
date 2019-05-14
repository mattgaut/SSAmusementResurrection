using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour, IInteractable {

    [SerializeField] Item to_spawn;
    bool opened;
    [SerializeField] Sprite open_sprite;

    [SerializeField] SFXInfo open_sfx = new SFXInfo("sfx_chest_open");

    public void SetSpawnItem(Item i) {
        to_spawn = i;
    }

    public void Interact(Player player) {
        if (!opened) {
            Item new_item = Instantiate(to_spawn);
            new_item.transform.position = transform.position;
            player.inventory.AddItem(new_item);
            opened = true;
            GetComponent<SpriteRenderer>().sprite = open_sprite;
            SoundManager.instance.LocalPlaySfx(open_sfx);
        }
    }

    public void SetHighlight(bool is_highlighted) {
        
    }
}
