using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombAAEffect : ActiveAbilityEffect {

    [SerializeField] Attack smoke_bomb_attack;
    [SerializeField] ParticleSystem blind_particle_effects;

    [SerializeField] float blind_length, radius;
    [SerializeField] float drop_probability;

    RNG rng;

    protected override void UseAbilityEffect(float input) {
        DropSmokeBomb();
    }

    private void Awake() {
        rng = new RNG();
    }

    void DropSmokeBomb() {
        Attack new_smoke_bomb = Instantiate(smoke_bomb_attack);
        new_smoke_bomb.transform.position = character.transform.position;

        new_smoke_bomb.SetSource(character);
        new_smoke_bomb.SetOnHit(OnHit);
        new_smoke_bomb.GetComponent<CircleCollider2D>().radius = radius;
        new_smoke_bomb.Enable();
    }

    void OnHit(Character hit, Attack a) {
        hit.crowd_control_effects.ApplyCC(CrowdControl.Type.blinded, blind_length, character);

        ParticleSystem ps = Instantiate(blind_particle_effects, hit.stats.head);
        ps.transform.localPosition = Vector3.zero;

        StartCoroutine(StopParticlesAfterTime(ps, blind_length));
    }

    IEnumerator StopParticlesAfterTime(ParticleSystem ps, float length) {
        yield return new WaitForSeconds(length);
        if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
