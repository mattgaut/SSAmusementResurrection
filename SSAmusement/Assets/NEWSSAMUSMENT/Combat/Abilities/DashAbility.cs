using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : ActiveAbility {

    [SerializeField] Vector2 base_dash;
    [SerializeField] bool modified_by_input;
    [SerializeField] float dash_time;

    IInputHandler input_handler;

    protected override void Awake() {
        base.Awake();
        input_handler = character.GetComponent<IInputHandler>();    
    }

    protected override void UseAbility() {
        Vector2 dash = base_dash;
        if (modified_by_input) {
            dash.x *= Mathf.Sign(input_handler.input.x != 0f ? input_handler.input.x : input_handler.facing);
        }

        character.Dash(dash, dash_time);
    }
}
