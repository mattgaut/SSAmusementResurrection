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

    SortedList<Character, Instance> unique_buffs;
    SortedList<int, Instance> applied_buffs;

    private int next_id = 0;

    public virtual int ApplyBuff(Character character, float length = 0, bool is_benificial = true, Sprite icon = null) {
        Instance new_buff;
        if (is_unique) {
            if (unique_buffs.ContainsKey(character)) {
                Instance active_buff = unique_buffs[character];
                //if (can_stack && max_stacks > ) {

                //}
                active_buff.Refresh();
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

        public int id { get; private set; }

        BuffController buff_group;
        List<BuffDefinition.PartialInstance> buffs;

        protected bool is_applied;

        public Instance(int id, BuffController buff_group) {
            this.id = id;
            this.buff_group = buff_group;       

            buffs = new List<BuffDefinition.PartialInstance>();
            foreach (BuffDefinition buff in buff_group.buffs) {
                buffs.Add(buff.GetPartialInstance());
            }
        }

        public void Apply(Character character) {
            if (is_applied) return;

            is_applied = true;
            buffed = character;
            foreach (BuffDefinition.PartialInstance instance in buffs) {
                instance.Apply(character);
            }
            if (length > 0) {
                character.LogBuff(this);
                character.StartCoroutine(RemoveAfter(length));
            }
        }

        public void Remove() {
            foreach (BuffDefinition.PartialInstance instance in buffs) {
                instance.Remove();
            }
            buff_group.RemoveFromAppliedBuffs(id);
            is_applied = false;
        }

        public void Refresh() {
            if (length > 0 && is_applied) {
                remaining_time = length;
            }
        }

        IEnumerator RemoveAfter(float time) {
            remaining_time = time;
            while (remaining_time > 0) {
                remaining_time -= GameManager.GetFixedDeltaTime(buffed.team);
                yield return new WaitForFixedUpdate();
            }
            Remove();
        }
    }
}
