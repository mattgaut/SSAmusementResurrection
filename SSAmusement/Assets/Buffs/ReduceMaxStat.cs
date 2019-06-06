using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceMaxStat : BuffDefinition {
    [SerializeField] float percent_reduction;
    public override BuffType type {
        get { return BuffType.stat; }
    }

    Dictionary<int, Stat.Modifier> id_to_buff_value_dict;


    protected override void Init() {
        base.Init();
        id_to_buff_value_dict = new Dictionary<int, Stat.Modifier>();
    }

    protected override void ApplyEffects(Character stat_entity, int id) {
        float amount_to_remove = stat_entity.health.flat_modded_value * percent_reduction;

        Stat.Modifier new_mod = new Stat.Modifier(-amount_to_remove);
        stat_entity.health.AddModifier(new_mod);
        id_to_buff_value_dict.Add(id, new_mod);
    }

    protected override void RemoveEffects(Character stat_entity, int id) {
        if (id_to_buff_value_dict.ContainsKey(id)) {
            stat_entity.character.health.RemoveModifier(id_to_buff_value_dict[id]);
            id_to_buff_value_dict.Remove(id);
        }
    }
}
