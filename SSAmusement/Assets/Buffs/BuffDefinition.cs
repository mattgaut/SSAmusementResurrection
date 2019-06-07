using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { stat, attack, healing, on_hit, tick, invincibility, crowd_control }

public abstract class BuffDefinition : MonoBehaviour {
    public abstract BuffType type { get; }

    private int next_id = 0;

    protected abstract void ApplyEffects(Character character, int id, IBuff info);
    protected abstract void RemoveEffects(Character character, int id);

    protected virtual void RecalculateEffects(int id, IBuff info) {

    }

    public ChildInstance GetChildInstance(IBuff parent) {
        return new ChildInstance(next_id++, this, parent);
    }

    public IBuff GetInstance(float length = 0, bool is_benificial = true, Sprite icon = null) {
        return new StandaloneInstance(next_id++, this, length, is_benificial, icon);
    }

    protected void Awake() {
        Init();
    }

    protected virtual void Init() {

    }

    public abstract class PartialInstance {
        public Character buffed { get; protected set; }
        public int id { get; private set; }
        public bool is_active { get; protected set; }

        protected BuffDefinition buff_definition { get; private set; }

        public PartialInstance(int id, BuffDefinition buff_definition) {
            this.id = id;
            this.buff_definition = buff_definition;
        }
    }

    public class ChildInstance : PartialInstance {
        IBuff parent;

        public ChildInstance(int id, BuffDefinition buff_definition, IBuff parent) : base(id, buff_definition) {
            this.parent = parent;
        }

        public void Apply(Character character) {
            if (is_active) return;

            is_active = true;
            buffed = character;
            buff_definition.ApplyEffects(buffed, id, parent);
        }

        public void Remove() {
            buff_definition.RemoveEffects(buffed, id);
            is_active = false;
            buffed = null;
        }

        public void RecalculateStacks() {
            buff_definition.RecalculateEffects(id, parent);
        }
    }

    protected class StandaloneInstance : PartialInstance, IBuff {
        public Sprite icon { get; private set; }
        public bool is_benificial { get; private set; }
        public float length { get; private set; }
        public float remaining_time { get; private set; }
        public int stack_count { get; private set; }

        bool is_timed;

        public StandaloneInstance(int id, BuffDefinition buff_definition, float length = 0, bool is_benificial = true, Sprite icon = null)
            : base(id, buff_definition) {
            this.icon = icon;
            this.is_benificial = is_benificial;
            this.length = length;

            stack_count = 1;

            is_timed = length > 0;
        }

        public void Apply(Character character) {
            if (is_active) return;

            is_active = true;
            buffed = character;
            buff_definition.ApplyEffects(buffed, id, this);
            if (is_timed) {
                character.LogBuff(this);
                buff_definition.StartCoroutine(RemoveAfter(length));
            }
        }

        public void Remove() {
            buff_definition.RemoveEffects(buffed, id);
            is_active = false;
            buffed = null;
        }

        public void AddStack() {
            stack_count++;
            buff_definition.RecalculateEffects(id, this);
        }
        public void AddStack(int i) {
            stack_count += i;
            buff_definition.RecalculateEffects(id, this);
        }
        public void RemoveStack() {
            stack_count--;
            buff_definition.RecalculateEffects(id, this);
        }
        public void RemoveStack(int i) {
            stack_count -= i;
            buff_definition.RecalculateEffects(id, this);
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