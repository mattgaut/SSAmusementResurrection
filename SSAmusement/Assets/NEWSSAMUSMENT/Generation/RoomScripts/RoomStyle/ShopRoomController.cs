using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoomController : RoomController {

    [SerializeField] RoomSet room_set;

    [SerializeField] int store_price;
    [SerializeField] ShopTerminal[] shop_terminals;
    [SerializeField][Range(0, 1)] float individual_sale_chance;

    public override RoomType room_type {
        get { return RoomType.shop; }
    }

    public override void Init() {
        base.Init();
        LoadShopTerminals();
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }

    void LoadShopTerminals() {
        List<Item> possible_items = ItemListSingleton.instance.GetAvailableItems();
        for (int i = 0; i < shop_terminals.Length; i++) {
            int price = (RNGSingleton.instance.item_rng.GetFloat() < individual_sale_chance) ? store_price : store_price / 2;
            shop_terminals[i].SetItemOnSale(possible_items[RNGSingleton.instance.item_rng.GetInt(0, possible_items.Count)], price);
            shop_terminals[i].on_purchase.AddListener(CloseAllTerminals);
        }
    }

    void CloseAllTerminals() {
        for (int i = 0; i < shop_terminals.Length; i++) {
            shop_terminals[i].CloseTerminal();
        }
    }
}
