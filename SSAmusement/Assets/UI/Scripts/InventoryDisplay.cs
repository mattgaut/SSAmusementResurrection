using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour {

    [SerializeField] GridLayoutGroup items_grid;
    [SerializeField] int items_per_row;
    [SerializeField] ItemDisplay item_display_prefab;

    [SerializeField] Dictionary<string, ItemDisplayCount> displays;

    private void Awake() {
        displays = new Dictionary<string, ItemDisplayCount>();
    }

    private void Start() {
        float width = items_grid.GetComponent<RectTransform>().rect.width;
        width -= items_grid.padding.horizontal + (items_grid.spacing.x * (items_per_row - 1));
        width = width / items_per_row;
        items_grid.cellSize = new Vector2(width, width);
    }

    public void AddItem(Item i) {
        if (displays.ContainsKey(i.item_name)) {
            displays[i.item_name].Increment();
        } else {
            ItemDisplay new_item_display = Instantiate(item_display_prefab, items_grid.transform);
            displays[i.item_name] = new ItemDisplayCount(i, new_item_display);
        }
    }

    public void RemoveItem(Item i) {
        if (displays.ContainsKey(i.item_name)) {
            displays[i.item_name].Decrement();
            if (displays[i.item_name].count <= 0) {
                Destroy(displays[i.item_name].display.gameObject);
                displays.Remove(i.item_name);
            }
        }
    }

    class ItemDisplayCount {
        public Item item;
        public ItemDisplay display;
        public int count;

        public ItemDisplayCount(Item i, ItemDisplay display) {
            item = i;
            this.display = display;
            count = 1;
            display.Display(i, count);
        }

        public void Increment() {
            display.Display(item, ++count);
        }

        public void Decrement() {
            display.Display(item, --count);
        }
    }
}
