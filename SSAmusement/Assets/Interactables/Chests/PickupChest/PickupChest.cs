using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupChest : MonoBehaviour {

    [SerializeField] List<Pickup> pickups_to_spawn;
    [SerializeField] Sprite open_sprite;
    [SerializeField] Transform spawn_point;
    [SerializeField] bool locked_to_player;

    [SerializeField] SFXInfo open_sfx = new SFXInfo("sfx_chest_open");

    bool opened;

    public void Open() {
        foreach (Pickup p in pickups_to_spawn) {
            SpawnAndThrowItems(p.gameObject);
        }
        opened = true;
        GetComponent<SpriteRenderer>().sprite = open_sprite;
        SoundManager.instance.LocalPlaySfx(open_sfx);
    }

    public void SetSpawnPickups(List<Pickup> pickups) {
        pickups_to_spawn = pickups;
    }

    public void Interact(Player player) {
        if (!opened && !locked_to_player) {
            Open();
        }
    }    

    public void SetHighlight(bool is_highlighted) {

    }

    void SpawnAndThrowItems(GameObject to_throw) {
        to_throw = Instantiate(to_throw);
        to_throw.transform.position = transform.position + Vector3.up * 0.5f;
        float angle = Random.Range(0f, 90f) - 45f;
        Rigidbody2D body = to_throw.GetComponent<Rigidbody2D>();
        body.AddForce(Quaternion.Euler(0, 0, angle) * Vector2.up * 8f, ForceMode2D.Impulse);
    }
}
