using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffGroup : MonoBehaviour, IBuff {
    public Sprite icon {
        get { return _icon; }
        set { _icon = value; }
    }
    public bool is_benificial {
        get { return _is_benificial; }
    }
    public float length {
        get { return remove_after; }
    }

    [SerializeField] List<BuffDefinition> buffs;

    [SerializeField] bool timed_buff;
    [SerializeField] float remove_after;
    [SerializeField] bool _is_benificial;
    [SerializeField] Sprite _icon;

    public void Apply(ICombatant stat_entity) {
        foreach (BuffDefinition buff in buffs) {
            buff.Apply(stat_entity);
        }
        if (timed_buff) {
            stat_entity.LogBuff(this);
            stat_entity.StartCoroutine(RemoveAfter(stat_entity, remove_after));
        }
    }

    public void Remove(ICombatant stat_entity) {
        foreach (BuffDefinition buff in buffs) {
            buff.Remove(stat_entity);
        }
    }

    IEnumerator RemoveAfter(ICombatant remove_from, float time) {
        float time_left = time;
        while (time_left > 0) {
            time_left -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Remove(remove_from);
    }
}
