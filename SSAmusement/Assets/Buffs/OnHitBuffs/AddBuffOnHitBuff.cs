using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBuffOnHitBuffInfo : OnHitBuffInfo {
    public int buff_id { get; set; }
    public AddBuffOnHitBuffInfo(IBuff buff) : base(buff) {
    }
}

public class AddBuffOnHitBuff : OnHitBuff<AddBuffOnHitBuffInfo> {

    [SerializeField] BuffController buff_to_apply;
    [SerializeField] bool should_remove_buffs;

    protected override void RemoveEffects(Character character, AddBuffOnHitBuffInfo info) {
        base.RemoveEffects(character, info);
        if (should_remove_buffs) {
            buff_to_apply.RemoveBuff(info.buff_id);
        }
    }

    protected override void OnHitEffect(AddBuffOnHitBuffInfo info, Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit) {
        info.buff_id = buff_to_apply.ApplyBuff(hitter, hit);
    }

    protected override AddBuffOnHitBuffInfo GetOnHitBuffInfo(IBuff buff) {
        return new AddBuffOnHitBuffInfo(buff);
    }
}
