using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : BuffDefinition {

    [SerializeField] bool health, energy, power, armor, speed;
    [SerializeField] Stat.Modifier modifier;

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    public void SetFlat(float flat) {
        modifier.flat = flat;
    }

    public void SetMulti(float multi) {
        modifier.multi = multi;
    }

    public void SetAffectedStats(bool health = false, bool energy = false, bool power = false, bool armor = false, bool speed = false) {
        this.health = health;
        this.energy = energy;
        this.power = power;
        this.armor = armor;
        this.speed = speed;
    }

    protected override void Apply(Character stat_entity, int id) {
        if (health) {
            stat_entity.health.AddModifier(modifier);
        }
        if (energy) {
            stat_entity.energy.AddModifier(modifier);
        }
        if (power) {
            stat_entity.power.AddModifier(modifier);
        }
        if (armor) {
            stat_entity.armor.AddModifier(modifier);
        }
        if (speed) {
            stat_entity.speed.AddModifier(modifier);
        }
    }

    protected override void Remove(Character stat_entity, int id) {
        if (health) {
            stat_entity.health.RemoveModifier(modifier);
        }
        if (energy) {
            stat_entity.energy.RemoveModifier(modifier);
        }
        if (power) {
            stat_entity.power.RemoveModifier(modifier);
        }
        if (armor) {
            stat_entity.armor.RemoveModifier(modifier);
        }
        if (speed) {
            stat_entity.speed.RemoveModifier(modifier);
        }
    }
}
