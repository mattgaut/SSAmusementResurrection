using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetCollector : MonoBehaviour {

    protected List<Character> targets;

    public virtual Character GetRandomTarget() {
        if (targets.Count > 0) {            
            return targets.GetRandom();
        }
        return null;
    }

    public virtual List<Character> GetTargetList() {
        return new List<Character>(targets);
    }

    protected void Awake() {
        targets = new List<Character>();
    }

    protected void RemoveTarget(Character character) {
        targets.Remove(character);
    }
}
