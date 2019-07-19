using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Formulas {
    public abstract class Formula : ScriptableObject {
        public abstract float GetValue();
    }

    public abstract class Formula<T1> : ScriptableObject {
        public abstract float GetValue(T1 t1);
    }

    public abstract class Formula<T1, T2> : ScriptableObject {
        public abstract float GetValue(T1 t1, T2 t2);
    }

    public abstract class Formula<T1, T2, T3> : ScriptableObject {
        public abstract float GetValue(T1 t1, T2 t2, T3 t3);
    }
}