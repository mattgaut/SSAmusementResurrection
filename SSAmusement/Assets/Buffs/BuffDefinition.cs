using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { stat, attack, healing, on_hit, tick, invincibility, crowd_control }

public abstract class BuffDefinition : MonoBehaviour {
    public abstract BuffType type { get; }

    private int next_id = 0;

    protected abstract void ApplyEffects(Character character, int id);
    protected abstract void RemoveEffects(Character character, int id);

    public PartialInstance GetPartialInstance() {
        return new PartialInstance(next_id++, this);
    }

    public IBuff GetInstance(float length = 0, bool is_benificial = true, Sprite icon = null) {
        return new Instance(next_id++, this, length, is_benificial, icon);
    }

    protected void Awake() {
        Init();
    }

    protected virtual void Init() {

    }

    public class PartialInstance {
        public Character buffed { get; protected set; }
        public int id { get; private set; }

        protected BuffDefinition buff_definition { get; private set; }
        protected bool is_applied { get; private set; }

        public PartialInstance(int id, BuffDefinition buff_definition) {
            this.id = id;
            this.buff_definition = buff_definition;
        }

        public virtual void Apply(Character character) {
            if (is_applied) return;

            is_applied = true;
            buffed = character;
            buff_definition.ApplyEffects(buffed, id);
        }

        public void Remove() {
            buff_definition.RemoveEffects(buffed, id);
            is_applied = false;
            buffed = null;
        }
    }

    protected class Instance : PartialInstance, IBuff {
        public Sprite icon { get; private set; }
        public bool is_benificial { get; private set; }
        public float length { get; private set; }
        public float remaining_time { get; private set; }

        bool is_timed;

        public Instance(int id, BuffDefinition buff_definition, float length = 0, bool is_benificial = true, Sprite icon = null)
            : base(id, buff_definition) {
            this.icon = icon;
            this.is_benificial = is_benificial;
            this.length = length;

            is_timed = length > 0;
        }

        public override void Apply(Character character) {
            base.Apply(character);
            if (is_timed) {
                character.LogBuff(this);
                buff_definition.StartCoroutine(RemoveAfter(length));
            }
        }

        IEnumerator RemoveAfter(float time) {
            remaining_time = time;
            while (remaining_time > 0) {
                remaining_time -= GameManager.GetFixedDeltaTime(buffed.team);
                yield return new WaitForFixedUpdate();
            }
            Remove();
        }
    }
}