using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChest : MonoBehaviour, IInteractable {

    [SerializeField] Item to_spawn;
    bool opened;
    [SerializeField] Sprite open_sprite;

    [SerializeField] Canvas canvas;
    [SerializeField] Image item_image;

    [SerializeField] SpriteRenderer chest_renderer;
    [SerializeField] SpriteRenderer shine_renderer;

    [SerializeField] SFXInfo open_sfx = new SFXInfo("sfx_chest_open");

    public void SetSpawnItem(Item i) {
        to_spawn = i;
    }

    public void Interact(Player player) {
        if (!opened) {
            item_image.sprite = to_spawn.icon;
            canvas.enabled = true;

            chest_renderer.sprite = open_sprite;
            shine_renderer.enabled = true;
            SoundManager.instance.LocalPlaySfx(open_sfx);

            opened = true;
        }
        else if (to_spawn != null) {
            player.inventory.AddItem(Instantiate(to_spawn));
            to_spawn = null;

            item_image.sprite = null;
            canvas.enabled = false;

            shine_renderer.enabled = false;
        }        
    }

    public void SetHighlight(bool is_highlighted) {
        
    }
}
