using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Statistic : MonoBehaviour {

    public enum Category { None, Combat, Items, Money, Meta }

    public abstract Category category {
        get;
    }

    public abstract new string name {
        get;
    }

    public abstract string string_value {
        get;
    }

    public abstract void Subscribe();

    public virtual void Unsubscribe() {

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

public abstract class SingleIntStatistic : Statistic {

    protected int count;

    public override string string_value {
        get { return count + ""; }
    }

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

public abstract class SingleFloatStatistic : Statistic {

    protected float count;

    public override string string_value {
        get { return (int)count + ""; }
    }

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