using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : Room {
    [SerializeField] ShopTerminal[] shop_terminals;

    public override Type room_type { get { return Type.shop; } }

    protected override void SpawnRandomRoomset() {

    }

    protected override void Start() {
        LoadShopTerminals();
    }

    void LoadShopTerminals() {
        List<Item> possible_items = ItemListSingleton.instance.GetAvailableItems();
        for (int i = 0; i < shop_terminals.Length; i++) {
            shop_terminals[i].SetItemOnSale(possible_items[RNGSingleton.instance.item_rng.GetInt(0, possible_items.Count)], 4);
            shop_terminals[i].on_purchase.AddListener(CloseAllTerminals);
        }
    }

    void CloseAllTerminals() {
        for (int i = 0; i < shop_terminals.Length; i++) {
            shop_terminals[i].CloseTerminal();
        }
    }
}
