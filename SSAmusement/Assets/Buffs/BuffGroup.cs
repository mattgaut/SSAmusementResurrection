using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffGroup : MonoBehaviour {
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

    public Instance GetIBuffInstance() {
        if (timed_buff)
            return new Instance(buffs, icon, is_benificial, remove_after);
        else
            return new Instance(buffs, icon, is_benificial, 0);
    }

    public class Instance : IBuff {
        public Character buffed { get; private set; }
        public Sprite icon { get; private set; }
        public bool is_benificial { get; private set; }
        public float length { get; private set; }

        private List<BuffDefinition.Instance> buffs;
        private bool has_been_applied;

        public Instance(List<BuffDefinition> definitions, Sprite icon, bool is_benificial, float length) {
            this.icon = icon;
            this.is_benificial = is_benificial;
            this.length = length;

            buffs = new List<BuffDefinition.Instance>();
            foreach (BuffDefinition buff in definitions) {
                buffs.Add(buff.GetInstance());
            }
        }

        public void Apply(Character character) {
            if (has_been_applied) return;

            has_been_applied = true;
            buffed = character;
            foreach (BuffDefinition.Instance instance in buffs) {
                instance.Apply(character);
            }
            if (length > 0) {
                character.LogBuff(this);
                character.StartCoroutine(RemoveAfter(length));
            }
        }

        public void Remove() {
            foreach (BuffDefinition.Instance instance in buffs) {
                instance.Remove();
            }
            has_been_applied = false;
        }

        IEnumerator RemoveAfter(float time) {
            float time_left = time;
            while (time_left > 0) {
                time_left -= GameManager.GetFixedDeltaTime(buffed.team);
                yield return new WaitForFixedUpdate();
            }
            Remove();
        }
    }
}
