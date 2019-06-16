using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Statistic : ScriptableObject {

    public enum Category { None, Combat, Items, Money, Meta }

    public bool is_active;

    public abstract Category category {
        get;
    }

    public abstract new string name {
        get;
    }

    public abstract string string_value {
        get;
    }

    public void Subscribe() {
        is_active = true;
        OnSubscribe();
    }

    public void Unsubscribe() {
        is_active = false;
        OnUnsubscribe();
    }

    public abstract Data Save();

    public abstract void Load(Data data);

    public void TryCombine(Statistic other) {
        if (GetType() != other.GetType()) {
            return;
        }
        Combine(other);
    }

    public abstract void Clear();

    protected abstract void OnSubscribe();
    protected virtual void OnUnsubscribe() { }

    protected virtual void Combine(Statistic other) {

    }
    protected abstract byte[] GetRawValue();

    [System.Serializable]
    public class Data {
        [SerializeField] public string name; 
        [SerializeField] public byte[] bytes;

        public Data(Statistic stat) {
            name = stat.name;
            bytes = stat.GetRawValue();
        }
    }
}

public abstract class NumericStatistic : Statistic {
    public virtual int value {
        get;
    }
    public event Action<int> on_value_changed;
    public sealed override string string_value {
        get { return value + ""; }
    }
    protected void InvokeOnValueChanged() {
        if (is_active) on_value_changed?.Invoke(value);
    }
}

public abstract class SingleIntStatistic : NumericStatistic {

    public override int value {
        get { return count; }
    }

    protected int count {
        get { return _count; }
        set {
            _count = value;
            InvokeOnValueChanged();
        }
    }
    int _count;

    public sealed override void Load(Data data) {
        count = BitConverter.ToInt32(data.bytes, 0);
    }

    public sealed override Data Save() {
        return new Data(this);
    }

    public sealed override void Clear() {
        count = 0;
    }

    protected sealed override void Combine(Statistic other) {        
        count += (other as SingleIntStatistic).count;
    }

    protected sealed override byte[] GetRawValue() {
        return BitConverter.GetBytes(count);
    }

    protected void Increment() {
        count++;
    }
    protected void Decrement() {
        count--;
    }
}

public abstract class SingleFloatStatistic : NumericStatistic {
    public override int value {
        get { return (int)count; }
    }

    protected float count {
        get { return _count; }
        set {
            _count = value;
            InvokeOnValueChanged();
        }
    }
    float _count;

    public sealed override void Load(Data data) {
        count = BitConverter.ToSingle(data.bytes, 0);
    }

    public sealed override Data Save() {
        return new Data(this);
    }

    public sealed override void Clear() {
        count = 0;
    }

    protected sealed override void Combine(Statistic other) {
        count += (other as SingleFloatStatistic).count;
    }

    protected sealed override byte[] GetRawValue() {
        return BitConverter.GetBytes(count);
    }
}