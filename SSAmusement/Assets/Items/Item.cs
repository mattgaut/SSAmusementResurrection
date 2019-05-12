using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [SerializeField] string _item_name;
    [SerializeField][TextArea(1, 3)] string _item_description;
    [SerializeField] Sprite sprite;
    [SerializeField] List<ItemEffect> effects;

    public string item_name {
        get { return _item_name; }
    }
    public string item_description {
        get { return _item_description; }
    }
    public Sprite icon {
        get { return sprite; }
    }

    public Player owner {
        get; private set;
    }

    void SetOwner(Player p) {
        owner = p;
    }

    public void OnPickup(Player p) {
        GetComponent<SpriteRenderer>().enabled = false;
        SetOwner(p);
        foreach (ItemEffect e in effects) {
            e.OnPickup(this);
        }
    }

    public void OnDrop(Player p) {
        if (p == owner) {
            foreach (ItemEffect e in effects) {
                e.OnDrop(this);
            }
            transform.parent.SetParent(null);
            SetOwner(null);
        }
    }

}
