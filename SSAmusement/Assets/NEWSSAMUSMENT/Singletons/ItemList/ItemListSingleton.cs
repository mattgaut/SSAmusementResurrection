using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListSingleton : Singleton<ItemListSingleton> {

    [SerializeField] List<Item> available_items;

    public List<Item> GetAvailableItems() {
        return new List<Item>(available_items);
    }

}
