using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    public static RoomManager instance {
        get; private set;
    }

    public RoomController active { get; private set; }

    [SerializeField] List<GameObject> objects_to_center_on_active_room;
    Dictionary<Room, List<Room>> rooms;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public void SetRooms(Dictionary<Room, List<Room>> rooms) {
        this.rooms = rooms;
    }

    public void SetActiveRoom(RoomController room_controller, bool set_focus = true) {
        if (room_controller != active) {
            if (active) {
                active.Deactivate();
            }
            active = room_controller;
            active.Activate();
            if (set_focus) UIHandler.FocusRoom(room_controller);
            if (active != null) {
                foreach (GameObject go in objects_to_center_on_active_room) {
                    go.transform.position = active.transform.position + new Vector3(active.room.size.x * Room.Section.width, active.room.size.y * Room.Section.height, 0)/2f;
                }
            }
        }
    }
}
