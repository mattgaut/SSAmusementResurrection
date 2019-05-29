﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public enum Type { passive, active }

    [SerializeField] string _item_name;
    [SerializeField][TextArea(1, 3)] string _item_description;
    [SerializeField] Sprite sprite;
    [SerializeField] List<ItemEffect> effects;

    public virtual Type item_type {
        get { return Type.passive; }
    }

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

    public virtual void OnPickup(Player p) {
        GetComponent<SpriteRenderer>().enabled = false;
        SetOwner(p);
        transform.localPosition = Vector3.zero;
        transform.SetParent(p.transform);
        foreach (ItemEffect e in effects) {
            e.OnPickup(this);
        }
    }

    public virtual void OnDrop(Player p) {
        if (p == owner) {
            foreach (ItemEffect e in effects) {
                e.OnDrop(this);
            }
            transform.SetParent(null);
            SetOwner(null);
        }
    }

    private void Awake() {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
