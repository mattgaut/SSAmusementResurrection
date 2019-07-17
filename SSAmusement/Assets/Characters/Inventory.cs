using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Inventory : MonoBehaviour {

    [SerializeField] ItemPedastal replaced_item_pedastal;

    Player player;
    SortedList<string, Item> items_in_inventory;
    
    public int currency {
        get; private set;
    }
    public int boss_keycards {
        get; private set;
    }
    public ReadOnlyCollection<Item> items {
        get { return new ReadOnlyCollection<Item>(items_in_inventory.Values); }
    }
    public ActiveItem active_item { get; private set; }
    public Consumeable consumeable { get; private set; }

    public event Action<Item> on_collect_item;
    public event Action<Item> on_drop_item;

    int pet_count;

    private void Awake() {
        //boss_keycards += 1;
        player = GetComponent<Player>();
        items_in_inventory = new SortedList<string, Item>();
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

    public void TryUseConsumable() {
        if (consumeable && consumeable.ability.TryUse()) {
            consumeable = null;
            player.player_display.SetConsumeableItemDisplay(consumeable);
        }
    }

    public int ItemCount(string name) {
        if (items_in_inventory.ContainsKey(name)) {
            return items_in_inventory[name].stack_count;
        }
        return 0;
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

    public void AddConsumeable(Consumeable new_consumeable) {
        if (new_consumeable != null) {
            if (consumeable != null) {
                consumeable.gameObject.SetActive(true);
                player.DropObject(consumeable.gameObject, true);
                Destroy(consumeable.gameObject);
            }

            consumeable = new_consumeable;
            consumeable.transform.position = transform.position;
            consumeable.transform.SetParent(transform);
            consumeable.gameObject.SetActive(false);

            player.player_display.SetConsumeableItemDisplay(consumeable);
        }
    }

    public Item AddItem(Item i, bool will_handle_old_item) {
        Item replaced_item = null;
        if (i.item_type == Item.Type.active) {
            replaced_item = active_item;
            active_item = i as ActiveItem;
            player.player_display.SetActiveItemDisplay(active_item.active_ability);
            if (replaced_item) {
                replaced_item.OnDrop(player);
                if (!will_handle_old_item) {
                    ItemPedastal new_pedastal = Instantiate(replaced_item_pedastal);
                    new_pedastal.transform.position = player.transform.position;
                    new_pedastal.SetItem(replaced_item, false);
                }
            }
        } else {
            if (items_in_inventory.ContainsKey(i.item_name)) {
                Destroy(i.gameObject);
                i = items_in_inventory[i.item_name];
            } else {
                items_in_inventory.Add(i.item_name, i);
            }
        }
        i.OnPickup(player);

        on_collect_item?.Invoke(i);

        return replaced_item;
    }

    public void AddKeycard(int i = 1) {
        if (i > 0) {
            boss_keycards += i;

            player.player_display.UpdateBossKey(boss_keycards);
        }
    }

    IEnumerator DestroyConsumableAfterAbilityOver(Consumeable consumeable) {
        while (consumeable.ability.is_using_ability) {
            yield return null;
        }
        Destroy(consumeable.gameObject);
    }

    void RemoveKeycard(int i = 1) {
        boss_keycards -= i;

        player.player_display.UpdateBossKey(boss_keycards);
    }
}
