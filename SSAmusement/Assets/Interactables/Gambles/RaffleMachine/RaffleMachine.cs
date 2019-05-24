using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RaffleMachine : MonoBehaviour, IInteractable {

    public bool is_available {
        get { return !paid_out; }
    }

    RNG rng;

    [SerializeField] int initial_cost;
    [SerializeField] int cost_growth;

    [SerializeField] Transform pickup_spawn_point;
    [SerializeField] Transform chest_spawn_point;

    [SerializeField] Text cost_text;

    [SerializeField] List<PayoutChance> payouts;

    [SerializeField] ItemPedastal pedastal;

    int text_size;

    int cost;

    bool paid_out = false;

    public void Interact(Player player) {
        if (paid_out) {
            return;
        }
        if (player.inventory.TrySpendCurrency(cost)) {
            Roll();
        }
    }

    public void SetHighlight(bool is_highlighted) {
        if (is_highlighted) {
            cost_text.fontSize = 2 * text_size;
        } else {
            cost_text.fontSize = text_size;
        }
    }

    void Roll() {
        Payout payout = rng.GetRandomChanceObject(payouts.Cast<IChanceObject<Payout>>().ToList());

        if (payout.type == Payout.Type.none) {
            cost += cost_growth;
            cost_text.text = "$" + cost;
        } else if (payout.type == Payout.Type.item) {
            ItemPedastal new_item = Instantiate(pedastal);
            new_item.transform.position = chest_spawn_point.position;

            new_item.SetItem(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.random_item_rng), true);

            PaidOut();
        } else if (payout.type == Payout.Type.pickups) {
            Utilities.Utilities.ThrowObjects(payout.pickups_to_spawn, pickup_spawn_point);

            PaidOut();
        }
    }

    void PaidOut() {
        paid_out = true;
        cost_text.enabled = false;
    }

    void Awake() {
        cost = initial_cost;
        rng = new RNG();
        text_size = cost_text.fontSize;
        cost_text.text = "$" + cost;
    }


    [System.Serializable]
    class PayoutChance : IChanceObject<Payout> {
        [SerializeField] Payout _value;
        [SerializeField] float _chance;

        public Payout value {
            get { return _value; }
        }

        public float chance {
            get { return _chance; }
        }
    }

    [System.Serializable]
    class Payout {
        public enum Type { none, pickups, item }
        public Type type;
        public List<GameObject> pickups_to_spawn;
    }
}
