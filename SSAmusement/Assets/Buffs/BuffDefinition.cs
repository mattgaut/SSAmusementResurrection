using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { stat, attack, healing, on_hit, tick, invincibility, crowd_control, on_kill }

public class BuffInfo {
    public IBuff buff { get; private set; }
    public BuffInfo(IBuff buff) {
        this.buff = buff;
    }
}

public interface IChildBuff {
    IBuff buff { get; }

    void Apply(Character character);
    void Remove();
    void RecalculateStacks();
}

public abstract class BuffDefinition<T> : BuffDefinition where T : BuffInfo {
    public override IChildBuff GetChildInstance(IBuff parent) {
        return new ChildInstance(this, parent);
    }

    public override IBuff GetInstance(float length = 0, bool is_benificial = true, Sprite icon = null) {
        return new StandaloneInstance(this, length, is_benificial, icon);
    }

    protected abstract void ApplyEffects(Character character, T info, IBuff buff);
    protected abstract void RemoveEffects(Character character, T info);

    protected virtual void RecalculateEffects(T info, IBuff buff) {

    }

    protected abstract T GetBuffInfo(IBuff buff);

    protected void Awake() {
        Init();
    }

    protected virtual void Init() {

    }

    protected abstract class PartialInstance {
        public bool is_active { get; protected set; }
        public abstract IBuff buff { get; }

        protected BuffDefinition<T> buff_definition { get; private set; }
        protected T info { get; private set; }

        public PartialInstance(BuffDefinition<T> buff_definition) {
            info = buff_definition.GetBuffInfo(buff);
            this.buff_definition = buff_definition;
        }

        protected PartialInstance(BuffDefinition<T> buff_definition, IBuff buff) {
            info = buff_definition.GetBuffInfo(buff);
            this.buff_definition = buff_definition;
        }
    }

    protected class ChildInstance : PartialInstance, IChildBuff {
        public override IBuff buff { get { return parent; } }

        IBuff parent;

        public ChildInstance(BuffDefinition<T> buff_definition, IBuff parent) : base(buff_definition, parent) {
            this.parent = parent;
        }

        public void Apply(Character character) {
            if (is_active) return;

            is_active = true;
            buff_definition.ApplyEffects(parent.buffed, info, parent);
        }

        public void Remove() {
            buff_definition.RemoveEffects(parent.buffed, info);
            is_active = false;
        }

        public void RecalculateStacks() {
            buff_definition.RecalculateEffects(info, parent);
        }
    }

    protected class StandaloneInstance : PartialInstance, IBuff {
        public override IBuff buff { get { return this; } }
        public Character buffed { get; protected set; }
        public Character source { get; protected set; }
        public Sprite icon { get; private set; }
        public bool is_benificial { get; private set; }
        public float length { get; private set; }
        public float remaining_time { get; private set; }
        public int stack_count { get; private set; }

        bool is_timed;

        public StandaloneInstance(BuffDefinition<T> buff_definition, float length = 0, bool is_benificial = true, Sprite icon = null)
            : base(buff_definition) {
            this.icon = icon;
            this.is_benificial = is_benificial;
            this.length = length;

            stack_count = 1;

            is_timed = length > 0;
        }

        public void Apply(Character affected, Character source) {
            if (is_active) return;

            is_active = true;
            buffed = affected;
            this.source = source;
            buff_definition.ApplyEffects(buffed, info, this);
            if (is_timed) {
                affected.LogBuff(this);
                buff_definition.StartCoroutine(RemoveAfter(length));
            }
        }

        public void Remove() {
            buff_definition.RemoveEffects(buffed, info);
            is_active = false;
            buffed = null;
        }

        public void AddStack() {
            SetStacks(stack_count + 1);
        }
        public void AddStack(int i) {
            SetStacks(stack_count + i);
        }
        public void RemoveStack() {
            SetStacks(stack_count - 1);
        }
        public void RemoveStack(int i) {
            SetStacks(stack_count - i);
        }
        public void SetStacks(int i) {
            stack_count = i;
            buff_definition.RecalculateEffects(info, this);
        }

        IEnumerator RemoveAfter(float time) {
            remaining_time = time;
            while (remaining_time > 0) {
                remaining_time -= GameManager.GetFixedDeltaTime(buffed.team);
                yield return new WaitForFixedUpdate();
            }
            if (is_active) Remove();
        }
    }
}