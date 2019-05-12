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

    float acceleration_grounded = 0f;
    float acceleration_airborne = 0f;

    float gravity;
    float jump_velocity;
    float max_jump_hold;
    float x_smooth;

    bool jumping, knocked_back_last_frame;

    Vector2 velocity;
    Vector2 gravity_force;

    Coroutine drop_routine;

    protected void ProcessSkillButton(float input, int skill_index) {
        if (abilities.HasAbility(skill_index)) {
            Ability ability = abilities.GetAbility(skill_index);
            if (ability.ability_type == Ability.Type.Active) {
                ability.active.TryUse(input);
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
            player.player_display.SetAbilityDisplay(abilities.GetAbility(i).active, i);
        }
    }

    private void Update() {
        if (GameManager.instance.input_active && player.can_input) {
            if (Input.GetButton("Attack")) {
                ProcessSkillButton(Input.GetAxis("Attack"), 0);
            }
            if (Input.GetButtonDown("Skill1")) {
                ProcessSkillButton(Input.GetAxis("Skill1"), 1);
            }
            if (Input.GetButtonDown("Skill2")) {
                ProcessSkillButton(Input.GetAxis("Skill2"), 2);
            }
        }

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Move();
    }

    void Move() {
        if (cont.collisions.above || cont.collisions.below) {
            gravity_force.y = 0;
            velocity.y = 0;
        }
        Vector2 adjusted_input = input;
        if (!player.can_input || !GameManager.instance.input_active || !player.can_move) {
            adjusted_input = Vector2.zero;
        }

        if (!jumping) gravity_force.y += gravity * Time.deltaTime;

        if (player.anti_gravity) { gravity_force.y = 0; }

        if (player.CheckCancelVelocityFlag()) {
            velocity = Vector3.zero;
            jumping = false;
            gravity_force.y = 0;
            cont.Move((velocity + gravity_force) * Time.deltaTime);
        } else if (!player.is_knocked_back) {
            if (player.is_dashing) {
                player.animator.SetBool("Running", false);
                player.animator.speed = 1f;

                cont.Move(player.dash_force);
                player.dash_force = Vector2.zero;
                velocity = Vector3.zero;
                gravity_force = Vector3.zero;
            } else {
                knocked_back_last_frame = false;
                if (GameManager.instance.input_active && player.can_input) {
                    if (adjusted_input.y <= -.99f && Input.GetButtonDown("Jump") && drop_routine == null && cont.OverPlatform()) {
                        if (cont.OverPlatform()) {
                            drop_routine = StartCoroutine(DropRoutine());
                        }
                    } else if (Input.GetButtonDown("Drop") && drop_routine == null && player.can_input) {
                        drop_routine = StartCoroutine(DropRoutine());
                    } else if (Input.GetButtonDown("Jump") && cont.collisions.below && player.can_move && player.can_input) {
                        gravity_force.y = 0;
                        velocity.y = jump_velocity;
                        StartCoroutine(JumpRoutine());
                    } else if (jumping) {
                        velocity.y = jump_velocity;
                    }
                } else if (jumping) {
                    velocity.y = jump_velocity;
                }

                float target_velocity_x = adjusted_input.x * player.speed;
                //velocity.x = Mathf.SmoothDamp(velocity.x, target_velocity_x, ref x_smooth, cont.collisions.below ? acceleration_grounded : acceleration_airborne);
                velocity.x = target_velocity_x;

                if (adjusted_input.x != 0 && (cont.collisions.below || cont.collisions.below_last_frame)) {
                    player.animator.SetBool("Running", true);
                    if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRun")) {
                        player.animator.speed = Mathf.Abs(velocity.x / 5f);
                    } else {
                        player.animator.speed = 1f;
                    }
                } else {
                    player.animator.SetBool("Running", false);
                    player.animator.speed = 1f;
                }

                if (player.can_change_facing) {
                    if (adjusted_input.x < 0) {
                        facing = -1;
                        flip_object.transform.localRotation = Quaternion.Euler(0, 180f, 0);
                    } else if (adjusted_input.x > 0) {
                        facing = 1;
                        flip_object.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }

                cont.Move((velocity + gravity_force) * Time.deltaTime);
            }
        } else {
            if (knocked_back_last_frame == false) gravity_force = Vector3.zero;
            knocked_back_last_frame = true;
            velocity.y = 0;
            cont.Move((gravity_force + player.knockback_force) * Time.deltaTime);
        }
        if (player.is_knocked_back && (cont.collisions.left || cont.collisions.right)) {
            player.CancelXKnockBack();
        }
        if (player.is_knocked_back && cont.collisions.above) {
            player.CancelYKnockBack();
        }
        if (player.is_knocked_back && cont.collisions.below) {
            player.CancelKnockBack();
        }
    }

    IEnumerator DropRoutine() {
        cont.RemovePlatformFromMask();
        float delay = .15f;
        while (delay > 0) {
            yield return new WaitForFixedUpdate();
            delay -= Time.deltaTime;
        }
        while (Input.GetButton("Drop")) {
            yield return new WaitForFixedUpdate();
        }
        cont.AddPlatformToMask();
        drop_routine = null;
    }

    IEnumerator JumpRoutine() {
        jumping = true;
        float time_left = max_jump_hold;
        bool held = true;
        while (time_left > 0 && held && !cont.collisions.above) {
            time_left -= Time.fixedDeltaTime;
            held = held && Input.GetButton("Jump");
            yield return new WaitForFixedUpdate();
        }
        jumping = false;
    }
}
