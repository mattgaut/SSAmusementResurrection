using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChest : MonoBehaviour, IInteractable {

    [SerializeField] Item to_spawn;
    Item replaced_item;
    bool opened;
    [SerializeField] Sprite open_sprite;

    [SerializeField] Canvas canvas;
    [SerializeField] Image item_image;

    [SerializeField] SpriteRenderer chest_renderer;
    [SerializeField] SpriteRenderer shine_renderer;

    [SerializeField] SFXInfo open_sfx = new SFXInfo("sfx_chest_open");

    public bool is_available { get { return to_spawn != null || !opened || replaced_item != null; } }

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
            replaced_item = player.inventory.AddItem(Instantiate(to_spawn), true);
            to_spawn = null;
             
            if (replaced_item) {
                replaced_item.transform.SetParent(transform);
                item_image.sprite = replaced_item.icon;
            } else {
                item_image.sprite = null;
                canvas.enabled = false;

                shine_renderer.enabled = false;
            }
        } else if (replaced_item != null) {
            replaced_item = player.inventory.AddItem(replaced_item, true);

            if (replaced_item) {
                replaced_item.transform.SetParent(transform);
                item_image.sprite = replaced_item.icon;
            }
        }     
    }

    public void SetHighlight(bool is_highlighted) {
        
    }
}
