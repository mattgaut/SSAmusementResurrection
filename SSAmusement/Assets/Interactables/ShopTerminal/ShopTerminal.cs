using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ShopTerminal : MonoBehaviour, IInteractable {

    public UnityEvent on_purchase { get; private set; }

    public bool is_available { get { return on_sale != null && !used; } }

    [SerializeField] int price;
    [SerializeField] Item on_sale;
    [SerializeField] Text sale_price_text;
    [SerializeField] Image item_image_display;
    [SerializeField] SoundEffects sfxs;

    int text_size;

    bool used;

    public void CloseTerminal() {
        used = true;
        sale_price_text.enabled = false;
        item_image_display.enabled = false;
    }

    public void SetItemOnSale(Item i, int p) {
        on_sale = i;
        price = p;

        sale_price_text.text = "$" + p;
        item_image_display.sprite = on_sale.icon;
    }

    public void Interact(Player player) {
        SellItem(player);
    }

    private void Awake() {
        if (on_sale != null) SetItemOnSale(on_sale, price);
        used = false;
        on_purchase = new UnityEvent();
        text_size = sale_price_text.fontSize;
    }

    private void SellItem(Player player) {
        if (on_sale != null && !used && player.inventory.TrySpendCurrency(price)) {
            Item new_item = Instantiate(on_sale);
            player.inventory.AddItem(new_item, false);

            SoundManager.instance?.LocalPlaySfx(sfxs.on_successful_purchase);

            on_purchase.Invoke();
        } else {
            SoundManager.instance?.LocalPlaySfx(sfxs.on_failed_purchase);
        }
    }

    public void SetHighlight(bool is_highlighted) {
        if (is_highlighted) {
            sale_price_text.fontSize = 2 * text_size;
        } else {
            sale_price_text.fontSize = text_size;
        }
    }

    [System.Serializable]
    class SoundEffects {
        public SFXInfo on_successful_purchase = new SFXInfo(SoundBankCodenames.sfx_purchase_success);
        public SFXInfo on_failed_purchase = new SFXInfo(SoundBankCodenames.sfx_purchase_failed);
    }
}
