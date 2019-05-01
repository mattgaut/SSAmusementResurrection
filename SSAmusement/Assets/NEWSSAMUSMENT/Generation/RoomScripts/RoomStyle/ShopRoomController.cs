using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoomController : RoomController {

    [SerializeField] RoomSet room_set;

    [SerializeField] ShopTerminal[] shop_terminals;

    public override RoomType room_type {
        get { return RoomType.shop; }
    }

    public override RoomSet GetRoomsetToLoad() {
        return room_set;
    }

    public override void Init() {
        base.Init();
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
