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
        for (int i = 0; i < shop_terminals.Length; i++) {
            int price = (RNGSingleton.instance.room_gen_rng.GetFloat() < individual_sale_chance) ? store_price / 2 : store_price;
            shop_terminals[i].SetItemOnSale(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.item_rng), price);
            shop_terminals[i].on_purchase.AddListener(CloseAllTerminals);
        }
    }

    void CloseAllTerminals() {
        for (int i = 0; i < shop_terminals.Length; i++) {
            shop_terminals[i].CloseTerminal();
        }
    }
}
