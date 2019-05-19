using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TickEffectBuff : BuffDefinition {
    public override BuffType type {
        get {
            return BuffType.tick;
        }
    }

    [SerializeField] float tick_rate;
    Dictionary<Character, List<Coroutine>> tick_routines;

    public void Awake() {
        tick_routines = new Dictionary<Character, List<Coroutine>>();
    }

    public override void Apply(Character stat_entity) {
        if (!tick_routines.ContainsKey(stat_entity)) {
            tick_routines.Add(stat_entity, new List<Coroutine>());
        }
        tick_routines[stat_entity].Add(StartCoroutine(Tick(stat_entity)));
    }

    public override void Remove(Character stat_entity) {
        StopCoroutine(tick_routines[stat_entity][0]);
        tick_routines[stat_entity].RemoveAt(0);
    }

    IEnumerator Tick(Character stat_entity) {
        float timer = 0;
        while (true) {
            timer += Time.fixedDeltaTime;
            if (timer > tick_rate) {
                timer -= tick_rate;
                if (stat_entity == null || !stat_entity.alive) break;
                else TickEffect(stat_entity);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    protected abstract void TickEffect(Character stat_entity);
}
