using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatradeeTwinsHandler : GroundedEnemyHandler {

    public bool is_plates_next_attack {
        get { return next_ability == abilities.throw_plates; }
    }
    public bool is_knives_next_attack {
        get { return next_ability == abilities.throw_knives; }
    }
    public bool is_cyclone_next_attack {
        get { return next_ability == abilities.cyclone; }
    }

    protected bool can_throw_plates {
        get { return !twin.throwing_plates; }
    }
    protected bool can_throw_knives {
        get { return !twin.throwing_knives; }
    }

    [SerializeField] BossRoomInfo info;
    [SerializeField] MatradeeTwinsHandler twin;

    [SerializeField] MatradeeTwinsAbilitySet abilities;

    [SerializeField] float spin_target_range;

    bool throwing_plates, throwing_knives;

    Ability next_ability, last_ability;

    protected override void Start() {
        base.Start();

        abilities.throw_knives.SetCanUse(() => can_throw_knives);
        abilities.throw_plates.SetCanUse(() => can_throw_plates);
        abilities.SetCharacter(enemy);
    }

    IEnumerator Wander() {
        yield return new WaitForFixedUpdate();
        PickNextAttack();
    }

    IEnumerator Spin() {
        yield return MoveTo(target.transform, spin_target_range);

        if (abilities.cyclone.TryUse()) { 
            while (abilities.cyclone.winding_up) {
                yield return null;
            }

            float difference = target.transform.position.x - transform.position.x;
            while (abilities.cyclone.is_using_ability) {
                last_ability = abilities.cyclone;
                if (Mathf.Abs(difference) > 0.2f) {
                    _input.x = Mathf.Sign(difference)/2f;
                } else {
                    _input.x = 0;
                }
                yield return new WaitForFixedUpdate();
                difference = target.transform.position.x - transform.position.x;
            }
            _input.x = 0;
        }
    }

    IEnumerator ThrowKnives() {
        throwing_knives = true;

        yield return MoveTo(info.knives_transform);

        if (abilities.throw_knives.TryUse()) {
            last_ability = abilities.throw_knives;
            Face(-1);
            while (abilities.throw_knives.is_using_ability) {
                yield return new WaitForFixedUpdate();
            }
        }


        throwing_knives = false;
    }

    IEnumerator ThrowPlates() {
        throwing_plates = true;

        yield return MoveTo(info.plate_transform);
        
        if (abilities.throw_plates.TryUse()) {            
            last_ability = abilities.throw_plates;
            Face(1);
            while (abilities.throw_plates.is_using_ability) {
                yield return new WaitForFixedUpdate();
            }
        }

        throwing_plates = false;
    }

    void PickNextAttack() {
        List<Ability> to_try = new List<Ability>();
        for (int i = 0; i < abilities.count; i++) {
            Ability a = abilities.GetAbility(i);
            if (a.is_available) to_try.Add(a);
        }
        if (to_try.Count > 0) {
            next_ability = to_try.GetRandom();
        } else {
            next_ability = null;
        }
    }

    [System.Serializable]
    class BossRoomInfo {
        public Transform plate_transform;
        public Transform knives_transform;
    }
}
