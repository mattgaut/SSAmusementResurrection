using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { stat, attack, healing, on_hit, tick, invincibility, crowd_control }

public abstract class BuffDefinition : MonoBehaviour {
    public abstract BuffType type { get; }

    protected abstract void Apply(Character character, int id);
    protected abstract void Remove(Character character, int id);

    public Instance GetInstance() {
        return new Instance(this);
    }

    public FullInstance GetIBuffInstance(float length = 0, bool is_benificial = true, Sprite icon = null) {
        return new FullInstance(this, length, is_benificial, icon);
    }

    protected void Awake() {
        Init();
    }

    protected virtual void Init() {

    }

    public class Instance {
        protected static int next_id = 0;

        public Character buffed { get; protected set; }

        protected int id;
        protected BuffDefinition buff_definition;

        bool has_been_applied;

        public Instance(BuffDefinition buff_definition) {
            id = next_id++;
            this.buff_definition = buff_definition;
        }

        public virtual void Apply(Character character) {
            if (has_been_applied) return;

            has_been_applied = true;
            buffed = character;
            buff_definition.Apply(character, id);
        }

        public void Remove() {
            buff_definition.Remove(buffed, id);
            has_been_applied = false;
        }
    }

    public class FullInstance : Instance, IBuff {
        public Sprite icon { get; private set; }
        public bool is_benificial { get; private set; }
        public float length { get; private set; }        

        bool is_timed;

        public FullInstance(BuffDefinition buff_definition,  float length = 0, bool is_benificial = true, Sprite icon = null) 
            : base(buff_definition) {
            id = next_id++;
            this.icon = icon;
            this.is_benificial = is_benificial;
            this.length = length;

            this.buff_definition = buff_definition;

            is_timed = length > 0;
        }

        public override void Apply(Character character) {
            base.Apply(character);
            if (is_timed) {
                character.LogBuff(this);
                buff_definition.StartCoroutine(RemoveAfter(length));
            }
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