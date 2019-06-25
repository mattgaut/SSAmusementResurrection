using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Stat {
    [System.Serializable]
    public class Modifier {
        [SerializeField] float _flat;
        [SerializeField] float _multi;

        public float flat { get { return _flat; } private set { _flat = value; } }
        public float multi { get { return _multi; } private set { _multi = value; } }

        public event System.Action on_changed;

        public Modifier(float flat = 0f, float multi = 0f) {
            this.multi = multi;
            this.flat = flat;
        }

        public Modifier(Modifier other) {
            multi = other.multi;
            flat = other.flat;
        }

        public void Set(float flat, float multi) {
            this.multi = multi;
            this.flat = flat;
            on_changed?.Invoke();
        }
        public void Set(Modifier other) {
            multi = other.multi;
            flat = other.flat;
            on_changed?.Invoke();
        }
        public void SetMulti(float multi) {
            this.multi = multi;
            on_changed?.Invoke();
        }
        public void SetFlat(float flat) {
            this.flat = flat;
            on_changed?.Invoke();
        }

        public static Modifier operator *(Modifier modifier, float multiplier) {
            return new Modifier(modifier.flat * multiplier, modifier.multi * multiplier);
        }
    }

    [SerializeField] float base_value;
    List<Modifier> mods = new List<Modifier>();

    float last_calculated;
    bool changed = true;

    public float flat_modded_value {
        get {
            return GetFlatBuffedValue();
        }
    }

    public float unmodded_value {
        get {
            return GetBuffedValue();
        }
    }

    protected float value {
        get {
            if (changed) {
                last_calculated = GetBuffedValue();
                changed = false;
                return last_calculated;
            } else {
                return last_calculated;
            }
        }
    }

    public Stat() {

    }

    public Stat(float base_value) {
        this.base_value = base_value;
        mods = new List<Modifier>();

        changed = true;
    }

    public virtual void AddModifier(Modifier mod) {
        mods.Add(mod);
        NoteChange();
        mod.on_changed += NoteChange;
    }

    public virtual void RemoveModifier(Modifier mod) {
        mods.Remove(mod);
        NoteChange();
        mod.on_changed -= NoteChange;
    }

    public virtual void SetBaseValue(float f) {
        base_value = f;
        NoteChange();
    }

    protected virtual void NoteChange() {
        changed = true;
    }

    public static implicit operator float(Stat s) {
        return s.value;
    }

    protected virtual float GetBuffedValue() {
        float to_ret = base_value;
        foreach (Modifier mod in mods) {
            to_ret += mod.flat;
        }
        foreach (Modifier mod in mods) {
            to_ret *= 1 + mod.multi;
        }
        return to_ret;
    }

    protected virtual float GetFlatBuffedValue() {
        float to_ret = base_value;
        foreach (Modifier mod in mods) {
            to_ret += mod.flat;
        }
        return to_ret;
    }
}

[System.Serializable]
public class CapStat : Stat {

    float _current = 0;
    public float current {
        get { return _current; }
        set {
            _current = value;
            if (current < 0) {
                current = 0;
            } else if (current > this.value) {
                current = this.value;
            }
        }
    }

    public float max {
        get { return this; }
    }

    public float percent {
        get { return current / max; }
    }

    public bool is_max {
        get { return current == max; }
    }

    public CapStat() : base() {

    }

    protected override float GetBuffedValue() {
        float to_return = base.GetBuffedValue();
        return to_return < 0 ? 0 : to_return;
    }

    public CapStat(float base_value) : base(base_value) {
        current = base_value;
    }

    public override void AddModifier(Modifier mod) {
        base.AddModifier(mod);
    }

    public override void SetBaseValue(float f) {
        float value_before = value;
        base.SetBaseValue(f);
        float value_after = value;
        if (value_before < value_after) {
            current += value_after - value_before;
        }
    }
    protected override void NoteChange() {
        float value_before = value;
        base.NoteChange();
        float value_after = value;
        if (value_before < value_after) {
            current += value_after - value_before;
        }
        if (current > value) {
            current = value;
        }
    }
}
