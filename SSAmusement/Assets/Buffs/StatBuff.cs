using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : BuffDefinition {

    [SerializeField] protected bool health, energy, power, armor, speed;
    [SerializeField] protected Stat.Modifier modifier;

    protected Dictionary<int, Stat.Modifier> modifiers;

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

    protected override void Init() {
        base.Init();
        modifiers = new Dictionary<int, Stat.Modifier>();
    }

    protected override void ApplyEffects(Character character, int id, IBuff buff) {
        Stat.Modifier new_modifier = new Stat.Modifier(modifier);

        AddModifier(character, new_modifier);

        modifiers.Add(id, new_modifier);
    }

    protected override void RemoveEffects(Character character, int id) {
        if (modifiers.ContainsKey(id)) {
            RemoveModifier(character, modifiers[id]);
            modifiers.Remove(id);
        }
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

    protected override void RecalculateEffects(int id, IBuff info) {
        modifiers[id].Set(modifier * info.stack_count);
    }
}
