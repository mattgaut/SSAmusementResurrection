using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffDefinition : MonoBehaviour {
    public abstract BuffType type { get; }

    public abstract IChildBuff GetChildInstance(IBuff parent);
    public abstract IBuff GetInstance(float length = 0, bool is_benificial = true, Sprite icon = null);
}
