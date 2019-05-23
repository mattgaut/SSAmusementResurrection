using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : BuffDefinition, IStatBuff {

    [SerializeField] bool health, energy, power, armor, speed;
    [SerializeField] float _flat, _multi;

    public float flat {
        get { return _flat; }
    }
    public float multi {
        get { return _multi; }
    }

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    public void SetFlat(float flat) {
        _flat = flat;
    }

    public void SetMulti(float multi) {
        _multi = multi;
    }

    public void SetAffectedStats(bool health = false, bool energy = false, bool power = false, bool armor = false, bool speed = false) {
        this.health = health;
        this.energy = energy;
        this.power = power;
        this.armor = armor;
        this.speed = speed;
    }

    public override void Apply(Character stat_entity) {
        if (health) {
            stat_entity.health.AddBuff(this);
        }
        if (energy) {
            stat_entity.energy.AddBuff(this);
        }
        if (power) {
            stat_entity.power.AddBuff(this);
        }
        if (armor) {
            stat_entity.armor.AddBuff(this);
        }
        if (speed) {
            stat_entity.speed.AddBuff(this);
        }
    }

    public override void Remove(Character stat_entity) {
        if (health) {
            stat_entity.health.RemoveBuff(this);
        }
        if (energy) {
            stat_entity.energy.RemoveBuff(this);
        }
        if (power) {
            stat_entity.power.RemoveBuff(this);
        }
        if (armor) {
            stat_entity.armor.RemoveBuff(this);
        }
        if (speed) {
            stat_entity.speed.RemoveBuff(this);
        }
    }
}
