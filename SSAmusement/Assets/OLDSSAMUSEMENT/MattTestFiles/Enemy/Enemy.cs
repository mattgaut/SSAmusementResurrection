using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character, ICombatant {

    [SerializeField] Sprite _icon;
    public Sprite icon {
        get { return _icon; }
    }

    public Room home {
        get; private set;
    }

    System.Func<IEnumerator> die_function;

    [SerializeField] List<Pickup> drop_on_death;

    public void SetRoom(Room r) {
        home = r;
    }

    public void AddDropOnDeath(Pickup pickup_prefab_to_drop) {
        drop_on_death.Add(pickup_prefab_to_drop);
    }

    public void SetDieEvent(System.Func<IEnumerator> die_event) {
        die_function = die_event;
    }

    protected override void Die() {
        last_hit_by.GiveKillCredit(this);

        if (die_function != null) {
            StartCoroutine(BeforeDestroy());
        } else {
            OnDie();
        }
        alive = false;
    }

    void OnDie() {
        DropPickups();
        if (home) home.RemoveEnemy(this);
        Destroy(gameObject);
    }

    void DropPickups() {
        foreach (Pickup pickup in drop_on_death) {
            Pickup p = Instantiate(pickup, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            float angle = Random.Range(0f, 90f) - 45f;
            p.GetComponent<Rigidbody2D>().AddForce(Quaternion.Euler(0, 0, angle) * Vector2.up * 8f, ForceMode2D.Impulse);
        }
    }

    IEnumerator BeforeDestroy() {
        yield return die_function();
        OnDie();
    }
}
