﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager> {

    public RoomController active { get; private set; }

    public delegate void OnEnterRoom(RoomController entered, bool is_first_time);

    public event OnEnterRoom on_enter_room;

    [SerializeField] SpriteRenderer background_prefab;
    [SerializeField] List<GameObject> objects_to_center_on_active_room;
    List<GameObject> loaded_backgrounds;
    SpriteRenderer main_background;
    Dictionary<Room, List<Room>> rooms;

    HashSet<RoomController> discovered_rooms;

    public void LoadBackgrounds(Sprite background) {
        loaded_backgrounds = new List<GameObject>();
        foreach (GameObject go in objects_to_center_on_active_room) {
            loaded_backgrounds.Add(Instantiate(go));
        }
        main_background = Instantiate(background_prefab, Vector3.zero, Quaternion.identity);
        main_background.sprite = background;
    }

    public void SetRooms(Dictionary<Room, List<Room>> rooms) {
        this.rooms = rooms;
        discovered_rooms = new HashSet<RoomController>();
    }

    public void SetActiveRoom(RoomController room_controller, bool set_focus = true) {
        if (room_controller != active) {
            if (active) {
                active.Deactivate();
            }
            active = room_controller;
            active.Activate();
            if (set_focus) UIHandler.FocusRoom(room_controller);
            if (active != null && loaded_backgrounds != null) {
                foreach (GameObject go in loaded_backgrounds) {
                    go.transform.position = active.transform.position + new Vector3(active.room.size.x * Room.Section.width, active.room.size.y * Room.Section.height, 0)/2f;
                }

                on_enter_room?.Invoke(active, !discovered_rooms.Contains(active));
                discovered_rooms.Add(active);
            }
        }
    }

    public bool IsInActiveRoom(Vector2 position) {
        return active.room.bounds.Contains(position);
    }
}
