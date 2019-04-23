using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StateMachineController : MonoBehaviour {

    [SerializeField] StateMachine state_machine;
    [SerializeField] List<ParameterCallback> parameter_callback_list;
    [SerializeField] List<StateCoroutine> state_coroutine_list;

    StateMachine.Instance state_machine_instance;

    Dictionary<State, Func<IEnumerator>> state_coroutines;
    Dictionary<Parameter, Func<bool>> parameter_callbacks;

    protected Coroutine state_machine_routine;

    public bool active { get; private set; }

    public void SetActive(bool active) {
        if (this.active != active) {
            this.active = active;
            if (active) {
                Activate();
            } else {
                Deactivate();
            }
        }
    }

    protected virtual void Awake() {
        state_machine_instance = state_machine.GetStateMachineInstance();

        parameter_callbacks = new Dictionary<Parameter, Func<bool>>();
        foreach (ParameterCallback pc in parameter_callback_list) {
            pc.Init(this);
            parameter_callbacks.Add(pc.GetParameter(), pc.callback);
        }
        state_machine_instance.SetCallbacks(parameter_callbacks);

        state_coroutines = new Dictionary<State, Func<IEnumerator>>();
        foreach (StateCoroutine sc in state_coroutine_list) {
            sc.Init(this);
            state_coroutines.Add(sc.GetState(), sc.coroutine);
        }
    }

    protected IEnumerator StateMachineCoroutine() {
        while (active) {
            if (state_coroutines.ContainsKey(state_machine_instance.current_state)) {
                yield return state_coroutines[state_machine_instance.current_state].Invoke();
                state_machine_instance.TransitionUsingCallbacks();
            } else {
                Debug.LogError("No Routine exists for state "  + state_machine_instance.current_state.name);
                break;
            }
        }
    }

    protected virtual void Activate() {
        state_machine_instance = state_machine.GetStateMachineInstance();
        state_machine_instance.SetCallbacks(parameter_callbacks);

        state_machine_routine = StartCoroutine(StateMachineCoroutine());
    }
    protected virtual void Deactivate() {
        StopAllCoroutines();
    }

    [Serializable]
    class ParameterCallback {
        [SerializeField] Parameter parameter;
        [SerializeField] string callback_name;
        [SerializeField] int callback_index;
        [SerializeField] bool callback_is_field;

        public Func<bool> callback { get; private set; }

        public void Init(System.Object grab_from) {
            if (callback_is_field) {
                callback = () => (bool)grab_from.GetType().GetField(callback_name).GetValue(grab_from);
            } else {
                callback = Delegate.CreateDelegate(typeof(Func<bool>), grab_from, grab_from.GetType().GetMethod(callback_name)) as Func<bool>;
            }
        }

        public Parameter GetParameter() {
            return parameter;
        }
    }

    [Serializable]
    class StateCoroutine {
        [SerializeField] State state;
        [SerializeField] string coroutine_name;
        [SerializeField] int coroutine_index;

        public Func<IEnumerator> coroutine { get; private set; }

        public void Init(System.Object grab_from) {
            MethodInfo mi = grab_from.GetType().GetMethod(coroutine_name, BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
            coroutine = Delegate.CreateDelegate(typeof(Func<IEnumerator>), grab_from, mi) as Func<IEnumerator>;
        }

        public State GetState() {
            return state;
        }
    }
}