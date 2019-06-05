using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Player))]
public class PlayerInputHandler : MonoBehaviour, IInputHandler {

    [SerializeField] AbilitySet abilities;

    public Vector2 input { get; protected set; }

    public int facing { get; private set; }

    CharacterController cont;
    Player player;
    [SerializeField] GameObject flip_object;

    [SerializeField] [Range(0, 20)] float max_jump_height;
    [SerializeField] [Range(0, 20)] float min_jump_height;
    [SerializeField] [Range(0, 5)] float time_to_jump_apex;

    [SerializeField] float jump_buffer;
    [SerializeField] float skill_buffer;

    float acceleration_grounded = 0f;
    float acceleration_airborne = 0f;

    float gravity;
    float jump_velocity;
    float max_jump_hold;
    float x_smooth;

    int jumps_used;
    bool grounded_jump_used;

    bool jumping, knocked_back_last_frame;

    Vector2 velocity;
    Vector2 gravity_force;

    Coroutine drop_routine;

    public event Action<bool> on_jump;
    public event Action on_land;

    protected void ProcessSkillButton(float input, int skill_index) {
        if (abilities.HasAbility(skill_index)) {
            Ability ability = abilities.GetAbility(skill_index);
            if (ability.ability_type == Ability.Type.ActiveCooldown) {
                ability.active_cooldown.TryUse(input);
            }
        }
    }

    private void Awake() {
        cont = GetComponent<CharacterController>();
        player = GetComponent<Player>();

        gravity = -(2 * min_jump_height) / (time_to_jump_apex * time_to_jump_apex);
        jump_velocity = time_to_jump_apex * Mathf.Abs(gravity);
        max_jump_hold = (max_jump_height - min_jump_height) / jump_velocity;
    }

    private void Start() {
        abilities.SetCharacter(player);

        for (int i = 0; i < abilities.count; i++) {
            player.player_display.SetAbilityDisplay(abilities.GetAbility(i).active_cooldown, i);
        }

        player.on_take_knockback += (a, b, c) => gravity_force.y = 0;
    }

    private void Update() {
        if (GameManager.instance.input_active && player.can_input) {
            if (MyInput.GetButton("Attack")) {
                ProcessSkillButton(MyInput.GetAxis("Attack"), 0);
            }
            if (MyInput.GetButtonDown("Skill1", skill_buffer)) {
                ProcessSkillButton(MyInput.GetAxis("Skill1"), 1);
            }
            if (MyInput.GetButtonDown("Skill2", skill_buffer)) {
                ProcessSkillButton(MyInput.GetAxis("Skill2"), 2);
            }
            if (MyInput.GetButtonDown("Skill3")) {
                ProcessSkillButton(MyInput.GetAxis("Skill3"), 3);
            }
            if (MyInput.GetButtonDown("ActiveSkill")) {                
                if (player.inventory.active_item != null) {
                    player.inventory.active_item.active_ability.TryUse();
                }
            }
            if (MyInput.GetButtonDown("UseConsumable")) {
                player.inventory.TryUseConsumable();
            }
        }

        input = new Vector2(MyInput.GetAxis("Horizontal"), MyInput.GetAxis("Vertical"));
        Move();
    }

    void Move() {
        if (cont.collisions.above || cont.collisions.below) {
            gravity_force.y = 0;
            velocity.y = 0;
        }
        if (cont.collisions.below) {
            jumps_used = 0;
            grounded_jump_used = false;
        }

        Vector2 adjusted_input = input;
        if (!player.can_input || !GameManager.instance.input_active || !player.can_move) {
            adjusted_input = Vector2.zero;
        }

        if (!jumping) gravity_force.y += gravity * GameManager.GetDeltaTime(player.team);

        if (player.anti_gravity) { gravity_force.y = 0; }

        if (player.CheckCancelVelocityFlag()) {
            velocity = Vector3.zero;
            jumping = false;
            gravity_force.y = 0;
            cont.Move((velocity + gravity_force) * GameManager.GetDeltaTime(player.team));
        } else if (!player.is_knocked_back) {
            if (player.is_dashing) {
                HandleDashingInput();
            } else {
                if (jumping) {
                    velocity.y = jump_velocity;
                } else if (GameManager.instance.input_active && player.can_input) {
                    HandleYInput(adjusted_input.y);
                }

                HandleXInput(adjusted_input.x);

                cont.Move((velocity + gravity_force) * GameManager.GetDeltaTime(player.team));
            }
        } else {
            HandleKnockedBackInput();
        }

        if (player.is_knocked_back) {
            CheckCancelKnockback();
        }
    }

    void CheckCancelKnockback() {
        if (cont.collisions.below) {
            player.CancelKnockBack();
        } else {
            if (cont.collisions.left || cont.collisions.right) {
                player.CancelXKnockBack();
            }
            if (cont.collisions.above) {
                player.CancelYKnockBack();
            }
        }
    }

    void HandleKnockedBackInput() {
        velocity.y = 0;
        cont.Move(player.knockback_force + (gravity_force * GameManager.GetDeltaTime(player.team)));
        player.knockback_force = Vector2.zero;
    }

    void HandleDashingInput() {
        player.animator.SetBool("Running", false);
        player.animator.speed = 1f;

        cont.Move(player.dash_force);
        player.dash_force = Vector2.zero;
        velocity = Vector3.zero;
        gravity_force = Vector3.zero;
    }

    void HandleYInput(float y_input) {
        if (y_input <= -.5f  && drop_routine == null && cont.OverPlatform() && MyInput.GetButtonDown("Jump", jump_buffer)) {
            if (cont.OverPlatform()) {
                drop_routine = StartCoroutine(DropRoutine());
            }
        } else if (drop_routine == null && player.can_move && MyInput.GetButtonDown("Drop")) {
            drop_routine = StartCoroutine(DropRoutine());
        } else if (player.can_move && MyInput.GetButtonDown("Jump", jump_buffer)) {
            if (cont.collisions.below) {
                Jump(true);
            } else if (jumps_used < (player.jump_count - (grounded_jump_used ? 0 : 1))) {
                Jump(false);
            }
        }
    }

    void HandleXInput(float x_input) {
        velocity.x = x_input * player.speed;

        if (x_input != 0 && (cont.collisions.below || cont.collisions.below_last_frame)) {
            player.animator.SetBool("Running", true);
            if (player.animator.IsAnimInState("PlayerRun")) {
                player.animator.speed = Mathf.Abs(velocity.x / 5f);
            } else {
                player.animator.speed = 1f;
            }
        } else {
            player.animator.SetBool("Running", false);
            player.animator.speed = 1f;
        }

        if (player.can_change_facing) {
            if (x_input < 0) {
                facing = -1;
                flip_object.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            } else if (x_input > 0) {
                facing = 1;
                flip_object.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    IEnumerator DropRoutine() {
        cont.RemovePlatformFromMask();
        float delay = .15f;
        while (delay > 0) {
            yield return new WaitForFixedUpdate();
            delay -= Time.deltaTime;
        }
        while (MyInput.GetButton("Drop")) {
            yield return new WaitForFixedUpdate();
        }
        cont.AddPlatformToMask();
        drop_routine = null;
    }

    void Jump(bool grounded) {
        gravity_force.y = 0;
        velocity.y = jump_velocity;
        jumps_used += 1;
        on_jump?.Invoke(grounded);
        if (grounded) {
            StartCoroutine(JumpRoutine());
            grounded_jump_used = true;
        } else {
            StartCoroutine(ForceJumpRoutine(.08f));
        }
    }

    IEnumerator JumpRoutine() {
        jumping = true;
        float time_left = max_jump_hold;
        bool held = true;
        while (time_left > 0 && held && !cont.collisions.above) {
            time_left -= GameManager.GetFixedDeltaTime(player.team);
            held = held && MyInput.GetButton("Jump");
            yield return new WaitForFixedUpdate();
        }
        jumping = false;
    }

    IEnumerator ForceJumpRoutine(float force) {
        jumping = true;
        float time_left = force;
        while (time_left > 0 && !cont.collisions.above) {
            time_left -= GameManager.GetFixedDeltaTime(player.team);
            yield return new WaitForFixedUpdate();
        }
        jumping = false;
    }
}
