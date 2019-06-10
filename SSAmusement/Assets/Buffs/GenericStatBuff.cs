using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuffInfo : BuffInfo {
    public Stat.Modifier modifier { get; private set; }

    public StatBuffInfo(IBuff buff, Stat.Modifier modifier) : base(buff) {
        this.modifier = modifier;
    }
}

public abstract class StatBuff<T> : BuffDefinition<T> where T : StatBuffInfo {

    [SerializeField] protected bool health, energy, power, armor, speed;
    [SerializeField] protected bool fill_cap_stats;
    [SerializeField] protected Stat.Modifier modifier;

    public override BuffType type {
        get {
            return BuffType.stat;
        }
    }

    public void SetFlat(float flat) {
        modifier.SetFlat(flat);
    }

    public void SetMulti(float multi) {
        modifier.SetMulti(multi);
    }

    public void SetAffectedStats(bool health = false, bool energy = false, bool power = false, bool armor = false, bool speed = false) {
        this.health = health;
        this.energy = energy;
        this.power = power;
        this.armor = armor;
        this.speed = speed;
    }

    protected override void ApplyEffects(Character character, T info, IBuff buff) {
        AddModifier(character, info.modifier);
    }

    protected override void RemoveEffects(Character character, T info) {
        RemoveModifier(character, info.modifier);
    }

    protected override void RecalculateEffects(T info, IBuff buff) {
        info.modifier.Set(modifier * buff.stack_count);
    }

    protected void AddModifier(Character character, Stat.Modifier modifier) {
        if (health) {
            character.health.AddModifier(modifier);
        }
        if (energy) {
            character.energy.AddModifier(modifier);
        }
        if (power) {
            character.power.AddModifier(modifier);
        }
        if (armor) {
            character.armor.AddModifier(modifier);
        }
        if (speed) {
            character.speed.AddModifier(modifier);
        }
    }

    protected void RemoveModifier(Character character, Stat.Modifier modifier) {
        if (health) {
            character.health.RemoveModifier(modifier);
        }
        if (energy) {
            character.energy.RemoveModifier(modifier);
        }
        if (power) {
            character.power.RemoveModifier(modifier);
        }
        if (armor) {
            character.armor.RemoveModifier(modifier);
        }
        if (speed) {
            character.speed.RemoveModifier(modifier);
        }
    }
}
