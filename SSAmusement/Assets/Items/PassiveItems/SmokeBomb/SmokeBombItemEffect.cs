using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombItemEffect : OnTakeDamageItemEffect {

    [SerializeField] Attack smoke_bomb_attack;
    [SerializeField] ParticleSystem blind_particle_effects;

    [SerializeField] float blind_length, radius;
    [SerializeField] float drop_probability;

    RNG rng;

    private void Awake() {
        rng = new RNG();
    }

    protected override void OnTakeDamage(Character hit, float pre_damage, float post_damage, Character source) {
        if (rng.GetFloat() < drop_probability) DropSmokeBomb();
    }

    void DropSmokeBomb() {
        Attack new_smoke_bomb = Instantiate(smoke_bomb_attack);
        new_smoke_bomb.transform.position = item.owner.transform.position;

        new_smoke_bomb.SetSource(item.owner);
        new_smoke_bomb.SetOnHit(OnHit);
        new_smoke_bomb.GetComponent<CircleCollider2D>().radius = radius;
        new_smoke_bomb.Enable();
    }

    void OnHit(Character character, Attack a) {
        character.crowd_control_effects.ApplyCC(CrowdControl.Type.blinded, blind_length, item.owner);

        ParticleSystem ps = Instantiate(blind_particle_effects, character.stats.head);
        ps.transform.localPosition = Vector3.zero;

        StartCoroutine(StopParticlesAfterTime(ps, blind_length));
    }

    IEnumerator StopParticlesAfterTime(ParticleSystem ps, float length) {
        yield return new WaitForSeconds(length);
        if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
