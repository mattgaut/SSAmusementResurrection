using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickEffectBuffInfo : BuffInfo {
    public Coroutine tick_routine { get; set; }
    public TickEffectBuffInfo(IBuff buff) : base(buff) {

    }
}

public abstract class TickEffectBuff<T> : BuffDefinition<T> where T : TickEffectBuffInfo {
    public enum Timing { source, affected }

    public override BuffType type {
        get {
            return BuffType.tick;
        }
    }

    [SerializeField] float tick_rate;
    [SerializeField] Timing timing;

    protected override void ApplyEffects(Character character, T info, IBuff buff) {
        info.tick_routine = StartCoroutine(Tick(info));
    }

    protected override void RemoveEffects(Character character, T info) {
        StopCoroutine(info.tick_routine);
        info.tick_routine = null;
    }

    IEnumerator Tick(T info) {
        Character character = info.buff.buffed;
        float timer = 0;
        while (info.buff.is_active) {
            timer += Time.fixedDeltaTime * GetTimeFactor(info);
            if (timer > tick_rate) {
                timer -= tick_rate;
                if (character == null || !character.is_alive) {
                    break;
                } else {
                    for (int i = 0; i < info.buff.stack_count; i++) {
                        TickEffect(info);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    protected abstract void TickEffect(T info);
    
    private float GetTimeFactor(T info) {
        if (timing == Timing.source) {
            return 1;
        } else if (timing == Timing.affected) {
            return GameManager.instance.GetTeamTimeScale(info.buff.buffed.team);
        }
        return 1;
    } 
}