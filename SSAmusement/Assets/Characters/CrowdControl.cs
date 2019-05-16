using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdControl {
    public enum Type { stunned, silenced, snared, blinded }

    Dictionary<Type, List<CrowdControlInstance>> applied_crowd_control;

    public CrowdControl() {
        applied_crowd_control = new Dictionary<Type, List<CrowdControlInstance>>();
        foreach (Type t in System.Enum.GetValues(typeof(Type))) {
            applied_crowd_control.Add(t, new List<CrowdControlInstance>());   
        }
    }

    public void Update(float time_step) {
        foreach (Type t in System.Enum.GetValues(typeof(Type))) {
            for (int i = applied_crowd_control[t].Count - 1; i >= 0; i--) {
                applied_crowd_control[t][i].time_left -= time_step;
                if (applied_crowd_control[t][i].time_left <= 0) {
                    applied_crowd_control[t].RemoveAt(i);
                }
            }
        }
    }

    public void ApplyCC(Type type, float length, ICombatant source) {
        applied_crowd_control[type].Add(new CrowdControlInstance(type, length, source));
    }

    public void ClearCC(Type type) {
        applied_crowd_control[type].Clear();
    }

    public bool IsCCed(params Type[] types) {
        foreach (Type t in types) {
            if (applied_crowd_control[t].Count > 0) {
                return true;
            }
        }
        return false;
    }

    class CrowdControlInstance {
        public float length { get; private set; }
        public float time_left;
        public ICombatant source { get; private set; }
        public Type type { get; private set; }

        public CrowdControlInstance(Type type, float length, ICombatant source) {
            this.type = type;
            this.length = time_left = length;
            this.source = source;
        }
    }
}