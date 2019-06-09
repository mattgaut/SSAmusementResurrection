using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBuffOnKillBuff : BuffDefinition {
    public override BuffType type {
        get { return BuffType.on_kill; }
    }

    [SerializeField] BuffController buff_to_apply;
    [SerializeField] bool should_remove_buffs;

    Dictionary<int, AppliedOnKillBuff> applied_buffs;

    protected override void Init() {
        base.Init();
        applied_buffs = new Dictionary<int, AppliedOnKillBuff>();
    }

    protected override void ApplyEffects(Character character, int id, IBuff info) {
        if (!applied_buffs.ContainsKey(id)) {
            AppliedOnKillBuff new_buff = new AppliedOnKillBuff(character, (a, b) => AddOnKillStack(id), info);
            applied_buffs.Add(id, new_buff);
            character.on_kill += new_buff.on_kill_callback;
        }
    }

    void AddOnKillStack(int id) {
        int child_buff_id = buff_to_apply.ApplyBuff(applied_buffs[id].character, applied_buffs[id].parent_buff.stack_count);
        applied_buffs[id].child_buff_id = child_buff_id;
        
    }

    protected override void RemoveEffects(Character character, int id) {
        if (applied_buffs.ContainsKey(id)) {
            character.on_kill -= applied_buffs[id].on_kill_callback;
            if (should_remove_buffs) {
                Debug.Log(buff_to_apply.RemoveBuff(applied_buffs[id].child_buff_id));
            }
            applied_buffs.Remove(id);
        }
    }    

    class AppliedOnKillBuff {
        public Character character { get; private set; }
        public Character.OnKillCallback on_kill_callback { get; private set; }
        public IBuff parent_buff { get; private set; }
        public int child_buff_id;

        public AppliedOnKillBuff(Character character, Character.OnKillCallback on_kill_callback, IBuff buff) {
            this.character = character;
            this.on_kill_callback = on_kill_callback;
            parent_buff = buff;
        }
    }
}
