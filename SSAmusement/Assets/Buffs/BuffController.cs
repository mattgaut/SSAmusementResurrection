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

    public virtual int ApplyBuff(Character character) {
        Instance new_buff;
        if (is_unique) {
            if (unique_buffs.ContainsKey(character)) {
                Instance active_buff = unique_buffs[character];
                if (can_stack && max_stacks > active_buff.stack_count) {
                    active_buff.AddStack();
                    if (refreshes_on_new_stack) {
                        active_buff.Refresh();
                    }
                } else {
                    active_buff.Refresh();
                }
                return active_buff.id;
            } else {
                new_buff = new Instance(next_id++, this);
                unique_buffs.Add(character, new_buff);
            }
        } else {
            new_buff = new Instance(next_id++, this);
        }
        new_buff.Apply(character);
        applied_buffs.Add(new_buff.id, new_buff);
        return new_buff.id;
    }

    public virtual bool RemoveBuff(int id) {
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
        public Sprite icon { get { return buff_group.icon; } }
        public bool is_benificial { get { return buff_group.is_benificial; } }
        public float length { get { return buff_group.length; } }
        public float remaining_time { get; private set; }
        public bool is_active { get; private set; }
        public int stack_count { get; private set; }

        public int id { get; private set; }

        BuffController buff_group;
        List<BuffDefinition.ChildInstance> buffs;
        
        public Instance(int id, BuffController buff_group, int initial_stack_count = 1) {
            this.id = id;
            this.buff_group = buff_group;

            stack_count = initial_stack_count;

            buffs = new List<BuffDefinition.ChildInstance>();
            foreach (BuffDefinition buff in buff_group.buffs) {
                buffs.Add(buff.GetChildInstance(this));
            }
        }

        public void Apply(Character character) {
            if (is_active) return;

            is_active = true;
            buffed = character;
            foreach (BuffDefinition.ChildInstance instance in buffs) {
                instance.Apply(character);
            }
            if (length > 0) {
                character.LogBuff(this);
                character.StartCoroutine(Timer(length));
            }
        }

        public void Remove() {
            foreach (BuffDefinition.ChildInstance instance in buffs) {
                instance.Remove();
            }
            buff_group.RemoveFromAppliedBuffs(id);
            is_active = false;
        }

        public void AddStack() {
            stack_count++;
            foreach (BuffDefinition.ChildInstance buff in buffs) {
                buff.RecalculateStacks();
            }
        }
        public void AddStack(int i) {
            stack_count += i;
            foreach (BuffDefinition.ChildInstance buff in buffs) {
                buff.RecalculateStacks();
            }
        }
        public void RemoveStack() {
            stack_count--;
            foreach (BuffDefinition.ChildInstance buff in buffs) {
                buff.RecalculateStacks();
            }
        }
        public void RemoveStack(int i) {
            stack_count -= i;
            foreach (BuffDefinition.ChildInstance buff in buffs) {
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
