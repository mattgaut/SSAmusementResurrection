using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBuffOnKillBuffInfo : BuffInfo {
    public Character.OnKillCallback on_kill_callback { get; set; }
    public int buff_controller_id { get; set; }
    public ApplyBuffOnKillBuffInfo(IBuff info) : base(info) {
    }
} 

public class ApplyBuffOnKillBuff : BuffDefinition<ApplyBuffOnKillBuffInfo> {
    public override BuffType type {
        get { return BuffType.on_kill; }
    }

    [SerializeField] BuffController buff_to_apply;
    [SerializeField] bool should_remove_buffs;

    protected override void ApplyEffects(Character character, ApplyBuffOnKillBuffInfo info, IBuff buff) {
        character.on_kill += info.on_kill_callback;
    }

    protected override void RemoveEffects(Character character, ApplyBuffOnKillBuffInfo info) {
        character.on_kill -= info.on_kill_callback;
        if (should_remove_buffs) {
            buff_to_apply.RemoveBuff(info.buff_controller_id);
        }
    }

    protected override ApplyBuffOnKillBuffInfo GetBuffInfo(IBuff buff) {
        ApplyBuffOnKillBuffInfo info = new ApplyBuffOnKillBuffInfo(buff);
        info.on_kill_callback = (a, b) => AddOnKillStack(info);
        return info;
    }

    void AddOnKillStack(ApplyBuffOnKillBuffInfo info) {
        info.buff_controller_id = buff_to_apply.ApplyBuff(info.buff.buffed, info.buff.source, info.buff.stack_count);
    }
}
