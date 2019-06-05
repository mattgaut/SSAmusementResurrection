using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    [System.Serializable]
    public class Modifier {
        public float multi;
        public float flat;

        public Modifier(float flat = 0f, float multi = 0f) {
            this.multi = multi;
            this.flat = flat;
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

    float GetBuffedValue() {
        float to_ret = base_value;
        foreach (Modifier mod in mods) {
            to_ret += mod.flat;
        }
        foreach (Modifier mod in mods) {
            if (mod.multi != 0)
                to_ret *= mod.multi;
        }
        return to_ret;
    }

    float GetFlatBuffedValue() {
        float to_ret = base_value;
        foreach (Modifier mod in mods) {
            to_ret += mod.flat;
        }
        return to_ret;
    }

    public virtual void AddModifier(Modifier mod) {
        mods.Add(mod);
        changed = true;
    }

    public virtual void RemoveModifier(Modifier mod) {
        mods.Remove(mod);
        changed = true;
    }

    public virtual void SetBaseValue(float f) {
        base_value = f;
        changed = true;
    }

    public static implicit operator float(Stat s) {
        return s.value;
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

    public override void AddModifier(Modifier mod) {
        float value_before = value;
        base.AddModifier(mod);
        float value_after = value;
        if (value_before < value_after) {
            current += value_after - value_before;
        }
        if (current > value) {
            current = value;
        }
    }
    public override void RemoveModifier(Modifier mod) {
        base.RemoveModifier(mod);
        if (current > value) {
            current = value;
        }
    }
    public override void SetBaseValue(float f) {
        float value_before = value;
        base.SetBaseValue(f);
        float value_after = value;
        if (value_before < value_after) {
            current += value_after - value_before;
        }
        if (current > value) {
            current = value;
        }
    }
}
