using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { stat, attack, healing, on_hit, tick, invincibility }

public abstract class BuffDefinition : MonoBehaviour {
    public abstract BuffType type { get; }    

    public abstract void Apply(ICombatant stat_entity);

    public abstract void Remove(ICombatant stat_entity);

    public Instance GetBuffInstance(float length = 0, bool is_benificial = true, Sprite icon = null) {
        return new Instance(this, length, is_benificial, icon);
    }

    protected void Awake() {
        Init();
    }

    protected virtual void Init() {

    }

    public class Instance : IBuff {
        public Sprite icon { get; private set; }
        public bool is_benificial { get; private set; }
        public float length { get; private set; }

        public BuffDefinition buff_definition { get; private set; }

        bool is_timed;

        public Instance(BuffDefinition buff_definition,  float length = 0, bool is_benificial = true, Sprite icon = null) {
            this.icon = icon;
            this.is_benificial = is_benificial;
            this.length = length;

            this.buff_definition = buff_definition;

            is_timed = length > 0;
        }

        public void Apply(ICombatant stat_entity) {
            buff_definition.Apply(stat_entity);
            if (is_timed) {
                stat_entity.LogBuff(this);
                stat_entity.StartCoroutine(RemoveAfter(stat_entity, length));
            }
        }
        public void Remove(ICombatant stat_entity) {
            buff_definition.Remove(stat_entity);
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
}