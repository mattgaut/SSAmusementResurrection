using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Inventory : MonoBehaviour {

    Player player;
    List<Item> items_in_inventory;
    
    public int currency {
        get; private set;
    }
    public int boss_keycards {
        get; private set;
    }
    public ReadOnlyCollection<Item> items {
        get { return new ReadOnlyCollection<Item>(items_in_inventory); }
    }
    public ActiveItem active_item { get; private set; }

    int pet_count;

    private void Awake() {
        //boss_keycards += 1;
        player = GetComponent<Player>();
        items_in_inventory = new List<Item>();
    }

    public bool TrySpendCurrency(int to_spend) {
        if (to_spend > currency) {
            return false;
        }
        currency -= to_spend;

        player.player_display.UpdateCurrencyText(currency);
        return true;
    }

    public bool TrySpendKeycard() {
        if (boss_keycards >= 1) {
            RemoveKeycard();
            return true;
        }
        return false;
    }

    public void AddCurrency(int to_add) {
        currency += to_add;

        player.player_display.UpdateCurrencyText(currency);
    }

    public int ItemCount(string name) {
        int count = 0;
        foreach (Item i in items_in_inventory) {
            if (i.item_name == name) count++;
        }
        return count;
    }

    public int PetCount() {
        return pet_count;
    }

    public void AddPet() {
        pet_count++;
    }
    public void RemovePet() {
        pet_count--;
    }

    public Item AddItem(Item i) {
        Item replaced_item = null;
        if (i.item_type == Item.Type.active) {
            replaced_item = active_item;
            active_item = i as ActiveItem;
        } else {
            items_in_inventory.Add(i);
        }
        i.transform.SetParent(transform);
        i.OnPickup(player);
        UIHandler.DisplayItem(i);

        return null;
    }

    public void AddKeycard(int i = 1) {
        if (i > 0) {
            boss_keycards += i;

            player.player_display.UpdateBossKey(boss_keycards);
        }
    }

    void RemoveKeycard(int i = 1) {
        boss_keycards -= i;

        player.player_display.UpdateBossKey(boss_keycards);
    }
}
