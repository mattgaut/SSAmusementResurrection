using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour, IInteractable {

    [SerializeField] Item to_spawn;
    bool opened;
    [SerializeField] Sprite open_sprite;

    void Open() {
        Item new_item = Instantiate(to_spawn);
        new_item.transform.position = transform.position;
        opened = true;
        GetComponent<SpriteRenderer>().sprite = open_sprite;
    }

    public void SetSpawnItem(Item i) {
        to_spawn = i;
    }

    public void Interact(Player player) {
        Open();
    }
}
