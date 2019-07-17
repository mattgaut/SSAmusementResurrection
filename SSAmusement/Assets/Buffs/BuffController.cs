using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour {
    public Sprite icon {
        get { return _icon; }
        set { _icon = value; }
    }
    public bool is_benificial {
        get { return _is_benificial; }
    }
    public float length {
        get { return remove_after; }
    }

    [SerializeField] List<BuffDefinition> buffs;

    [SerializeField] bool timed_buff;
    [SerializeField] float remove_after;
    [SerializeField] bool _is_benificial;
    [SerializeField] Sprite _icon;

    [SerializeField] bool is_unique;
    [SerializeField] bool can_stack;
    [SerializeField] bool refreshes_on_new_stack;
    [SerializeField] int max_stacks;
    [SerializeField] bool has_incremental_falloff;

    SortedList<Character, Instance> unique_buffs;
    SortedList<int, Instance> applied_buffs;

    private int next_id = 0;

    public bool IsApplied(int buff_id) {
        return applied_buffs.ContainsKey(buff_id);
    }

    public int ApplyBuff(Character affected, Character source, int stacks, bool force_log_buff = false) {
        if (!is_unique || !can_stack) return ApplyBuff(affected, source, force_log_buff);
        int last_id = -1;
        for (int i = 0; i < stacks; i++) {
            last_id = ApplyBuff(affected, source, force_log_buff);
        }
        return last_id;
    }

    public int ApplyBuff(Character affected, Character source, bool force_log_buff = false) {
        Instance new_buff;
        if (is_unique) {
            if (unique_buffs.ContainsKey(affected)) {
                Instance active_buff = unique_buffs[affected];
                if (can_stack && (max_stacks > active_buff.stack_count || max_stacks <= 0)) {
                    active_buff.AddStack();
                    if (timed_buff && refreshes_on_new_stack) {
                        active_buff.Refresh();
                    }
                } else if (timed_buff) {
                    active_buff.Refresh();
                }
                return active_buff.id;
            } else {
                new_buff = new Instance(next_id++, this);
                unique_buffs.Add(affected, new_buff);
            }
        } else {
            new_buff = new Instance(next_id++, this);
        }

        new_buff.Apply(affected, source);
        if (force_log_buff && new_buff.length <= 0) {
            affected.LogBuff(new_buff);
        }
        applied_buffs.Add(new_buff.id, new_buff);
        return new_buff.id;
    }

    public bool RemoveBuff(int id) {
        if (applied_buffs.ContainsKey(id)) {
            applied_buffs[id].Remove();
            return true;
        }
        return false;
    }

    private void RemoveFromAppliedBuffs(int id) {
        if (is_unique) {
            unique_buffs.Remove(applied_buffs[id].buffed);
        }
        applied_buffs.Remove(id);
    }

    private void Awake() {
        unique_buffs = new SortedList<Character, Instance>();
        applied_buffs = new SortedList<int, Instance>();
    }

    protected class Instance : IBuff {
        public Character buffed { get; private set; }
        public Character source { get; private set; }
        public Sprite icon { get { return buff_group.icon; } }
        public bool is_benificial { get { return buff_group.is_benificial; } }
        public float length { get { return buff_group.length; } }
        public float remaining_time { get; private set; }
        public bool is_active { get; private set; }
        public int stack_count { get; private set; }

        public int id { get; private set; }

        BuffController buff_group;
        List<IChildBuff> buffs;
        
        public Instance(int id, BuffController buff_group, int initial_stack_count = 1) {
            this.id = id;
            this.buff_group = buff_group;

            is_active = false;

            stack_count = initial_stack_count;

            buffs = new List<IChildBuff>();
            foreach (BuffDefinition buff in buff_group.buffs) {
                buffs.Add(buff.GetChildInstance(this));
            }
        }

        public void Apply(Character affected, Character source) {
            if (is_active) return;

            is_active = true;
            buffed = affected;
            this.source = source;
            foreach (IChildBuff instance in buffs) {
                instance.Apply(affected);
            }
            if (length > 0) {
                affected.LogBuff(this);
                affected.StartCoroutine(Timer(length));
            }
        }

        public void Remove() {
            foreach (IChildBuff instance in buffs) {
                instance.Remove();
            }
            buff_group.RemoveFromAppliedBuffs(id);
            is_active = false;
        }

        public void AddStack() {
            SetStacks(stack_count + 1);
        }
        public void AddStack(int i) {
            SetStacks(stack_count + i);
        }
        public void RemoveStack() {
            SetStacks(stack_count - 1);
        }
        public void RemoveStack(int i) {
            SetStacks(stack_count - i);
        }
        public void SetStacks(int i) {
            stack_count = i;
            foreach (IChildBuff buff in buffs) {
                buff.RecalculateStacks();
            }
        }

        public void Refresh() {
            if (length > 0 && is_active) {
                remaining_time = length;
            }
        }

        IEnumerator Timer(float time) {
            remaining_time = time;

            while (is_active) {
                while (remaining_time > 0) {
                    yield return new WaitForFixedUpdate();
                    remaining_time -= GameManager.GetFixedDeltaTime(buffed.team);
                }
                if (is_active) {
                    if (stack_count > 1 && buff_group.can_stack && buff_group.has_incremental_falloff) {
                        RemoveStack();
                        remaining_time += length;
                    } else {
                        Remove();
                    }
                }                
            }    
        }
    }
}
