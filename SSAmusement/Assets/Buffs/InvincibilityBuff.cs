using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityBuffInfo : BuffInfo {
    public int lock_number { get; set; }

    public InvincibilityBuffInfo(IBuff buff) : base(buff) {
    }
}

public class InvincibilityBuff : BuffDefinition<InvincibilityBuffInfo> {
    public override BuffType type {
        get { return BuffType.invincibility; }
    }

    protected override void ApplyEffects(Character character, InvincibilityBuffInfo info, IBuff buff) {
        info.lock_number = character.LockInvincibility();
    }

    protected override void RemoveEffects(Character character, InvincibilityBuffInfo info) {
        character.UnlockInvincibility(info.lock_number);
    }

    protected override InvincibilityBuffInfo GetBuffInfo(IBuff buff) {
        return new InvincibilityBuffInfo(buff);
    }
}
