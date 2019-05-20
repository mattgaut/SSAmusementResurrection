using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPedastal : MonoBehaviour, IInteractable {

    public bool is_available { get { return stored_item != null; } }

    [SerializeField] Image item_image;

    Item stored_item;

    public void Interact(Player player) {
        if (stored_item != null) {
            Item new_item = player.inventory.AddItem(stored_item, true);
            if (new_item != null) {
                SetItem(new_item, false);
            } else {
                stored_item = null;
                Destroy(gameObject);
            }
        }
    }

    public void SetHighlight(bool is_highlighted) {

    }

    public void SetItem(Item item, bool is_prefab) {
        if (is_prefab) {
            item = Instantiate(item);
        }

        item.transform.SetParent(transform);
        item_image.sprite = item.icon;

        stored_item = item;
    }
}
