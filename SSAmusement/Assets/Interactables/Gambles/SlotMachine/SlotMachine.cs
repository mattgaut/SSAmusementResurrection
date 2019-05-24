using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour, IInteractable {
    public bool is_available {
        get { return !is_rolling; }
    }

    RNG rng;

    bool is_rolling;

    [SerializeField] Image screen;
    [SerializeField] Sprite fail_sprite;
    [SerializeField] Sprite[] roll_possibilities;
    [SerializeField] Transform pickup_spawn_point;
    [SerializeField] int cost;

    public void Interact(Player player) {
        if (is_rolling) {
            return;
        }
        if (player.inventory.TrySpendCurrency(cost)) {
            StartCoroutine(Roll(1f));
        }
    }

    public void SetHighlight(bool is_highlighted) {

    }

    IEnumerator Roll(float length) {
        is_rolling = true;
        float timer = length;
        float image_timer = 0f;
        int roll_image = Random.Range(0, roll_possibilities.Length);
        screen.sprite = roll_possibilities[roll_image];
        while (timer > 0) {
            if (image_timer > .025f) {
                image_timer -= .04f;
                roll_image = (roll_image + 1) % roll_possibilities.Length;
                screen.sprite = roll_possibilities[roll_image];
            }

            yield return null;

            float time_step = GameManager.GetDeltaTime(null);
            timer -= time_step;
            image_timer += time_step;
        }

        List<Pickup> pickups = LootTablesSingleton.instance.slot_machine_loot.GetPilePickupsFromSameCategory(rng);
        if (pickups.Count > 0) {
            screen.sprite = pickups[0].GetComponent<SpriteRenderer>().sprite;
        } else {
            screen.sprite = fail_sprite;
        }
        Utilities.Utilities.ThrowObjects(pickups, pickup_spawn_point);

        is_rolling = false;
    }

    void Awake() {
        rng = new RNG();
    }
}
