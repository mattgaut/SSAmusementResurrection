using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActiveAbility : Ability {

    public bool is_using_ability {
        get {
            foreach (ActiveAbilityEffect e in effects) {
                if (e.is_using_ability)
                    return true;
            }
            return false;
        }
    }
    public override bool is_available {
        get {
            return can_use == null || can_use.Invoke(); 
        }
    }

    public int cost { get { return _cost; } }

    public UnityEvent on_ability_used { get { return _on_ability_used; } }

    public delegate bool CanUse();
    CanUse can_use;

    [SerializeField] int _cost;

    [SerializeField] ActiveAbilityEffect[] effects;
    [SerializeField][HideInInspector] UnityEvent _on_ability_used;

    public void SetCanUse(CanUse can_use) {
        this.can_use = can_use;
    }

    public bool TryUse(float input = 0) {
        if (is_available && TryPayCost(cost)) {
            NoteAbilityUsed();
            foreach (ActiveAbilityEffect e in effects) {
                e.TriggerEffect(character, input);
            }
            return true;
        } else {
            return false;
        }
    }

    protected abstract bool TryPayCost(int cost);

    protected abstract void OnAbilityUsed();

    void NoteAbilityUsed() {
        OnAbilityUsed();
        on_ability_used.Invoke();
    }
}
