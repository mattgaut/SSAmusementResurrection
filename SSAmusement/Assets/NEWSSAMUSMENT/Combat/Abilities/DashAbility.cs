using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : ActiveAbility {

    [SerializeField] Vector2 base_dash;
    [SerializeField] bool modified_by_input;
    [SerializeField] float dash_time;

    IInputHandler character_input_handler;

    protected override void Awake() {
        base.Awake();
        character_input_handler = character.GetComponent<IInputHandler>();    
    }

    protected override void UseAbility(float input) {
        Vector2 dash = base_dash;

        if (input != 0) {
            dash.x *= Mathf.Sign(input);
        } else if (modified_by_input) {
            dash.x *= Mathf.Sign(character_input_handler.input.x != 0f ? character_input_handler.input.x : character_input_handler.facing);
        }

        character.Dash(dash, dash_time);
    }
}
