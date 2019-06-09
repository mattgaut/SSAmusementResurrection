using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff : StatBuff<StatBuffInfo> {
    protected override StatBuffInfo GetBuffInfo(IBuff buff) {
        return new StatBuffInfo(buff, new Stat.Modifier(modifier));
    }
}
