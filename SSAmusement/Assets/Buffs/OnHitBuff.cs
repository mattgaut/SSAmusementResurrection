using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitBuffInfo : BuffInfo {
    public Character.OnHitCallback on_hit_callback { get; set; }

    public OnHitBuffInfo(IBuff buff) : base(buff) {

    }
}

public abstract class OnHitBuff<T> : BuffDefinition<T> where T : OnHitBuffInfo {
    public override BuffType type {
        get {
            return BuffType.on_hit;
        }
    }

    protected override void ApplyEffects(Character character, T info, IBuff buff) {
        character.on_landed_hit += info.on_hit_callback;
    }

    protected override void RemoveEffects(Character character, T info) {
        character.on_landed_hit -= info.on_hit_callback;
    }

    protected sealed override T GetBuffInfo(IBuff buff) {
        T on_hit_buff_info = GetOnHitBuffInfo(buff);
        on_hit_buff_info.on_hit_callback = (hitter, pre, post, hit) => OnHitEffect(on_hit_buff_info, hitter, pre, post, hit);
        return on_hit_buff_info;
    }

    protected abstract T GetOnHitBuffInfo(IBuff buff);

    protected abstract void OnHitEffect(T info, Character hitter, float pre_mitigation_damage, float post_mitigation_damage, Character hit);
}
