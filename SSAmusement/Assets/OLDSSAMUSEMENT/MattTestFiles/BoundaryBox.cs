using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryBox : MonoBehaviour {
    [SerializeField] EdgeCollider2D left, right, bottom, top;

    public void Set(Room room) {
        Vector2Int width = new Vector2Int(Room.Section.width * room.size.x, 0);

        left.points = new Vector2[] { room.position, room.position + new Vector2Int(0, Room.Section.height * room.size.y) };
        right.points = new Vector2[] { left.points[0] + width, left.points[1] + width};
        top.points = new Vector2[] { left.points[1], right.points[1] };
        bottom.points = new Vector2[] { left.points[0], right.points[0] };
    }

    public void Disable() {
        SetEnabled(false);
    }

    public void Enable() {
        SetEnabled(true);
    }

    void SetEnabled(bool enabled) {
        left.enabled = right.enabled = top.enabled = bottom.enabled = enabled;
    }
}
