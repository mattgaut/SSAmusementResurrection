using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomProp : MonoBehaviour {
    [SerializeField] Sprite _sprite;

    public Sprite sprite {
        get { return _sprite; }
    }
}
