using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBomb : MonoBehaviour {
    [SerializeField] SingleHitAttack bomb_attack;
    [SerializeField] Rigidbody2D backup_rigidbody;
    [SerializeField] float time_to_detonation;
    [SerializeField] float active_hitbox_length;

    float timer = 0;

    Character attached_to;
    Vector3 offset;
    Character owner;

    Attack.OnHit on_hit;

    public void BeginBomb(Character source, Character attach_to, Attack.OnHit on_hit) {
        owner = source;

        attached_to = attach_to;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        SpriteRenderer other_renderer = attach_to.GetComponentInChildren<SpriteRenderer>();
        renderer.sortingLayerID = other_renderer.sortingLayerID;
        renderer.sortingOrder = other_renderer.sortingOrder + 1;

        offset = new Vector2(Random.Range(-.3f, .3f), Random.Range(-.3f, .3f));

        transform.position = attached_to.char_definition.center_mass.position + offset;

        this.on_hit = on_hit;
    }

    private void Update() {
        timer += GameManager.GetDeltaTime(owner?.team);

        if (timer > time_to_detonation) {
            Explode();
        }
    }

    private void LateUpdate() {
        if (attached_to) {
            transform.position = attached_to.char_definition.center_mass.position + offset;
        } else {
            backup_rigidbody.simulated = true;
        }
    }

    void Explode() {
        SingleHitAttack attack = Instantiate(bomb_attack);
        attack.SetSource(owner);
        attack.SetOnHit(on_hit);
        attack.Enable();
        attack.StartCoroutine(DisableAfter(attack, active_hitbox_length));

        attack.transform.position = transform.position;

        Destroy(gameObject);
    }

    IEnumerator DisableAfter(SingleHitAttack attack, float length) {
        while (length > 0) {
            yield return null;
            length -= GameManager.GetDeltaTime(attack.source?.team);
        }
        attack.Disable();
    }
}
