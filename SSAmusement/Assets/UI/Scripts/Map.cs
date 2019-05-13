﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    [SerializeField] MapObject map_object;
    [SerializeField] GameObject door_icon;
    [SerializeField] GameObject container;
    [SerializeField] float scale;

    [SerializeField] RoomIconMap icon_map;

    Dictionary<Room, MapObject> map;
    MapObject current_focus;

    public void Awake() {
        map = new Dictionary<Room, MapObject>();
        icon_map.Init();
    }

    public void TryAddRoom(RoomController room_controller) {
        Room r = room_controller.room;
        if (map.ContainsKey(r)) {
            return;
        }
        MapObject new_map_object = Instantiate(map_object);
        new_map_object.transform.SetParent(container.transform, false);
        new_map_object.SetToRoom(r, scale);
        new_map_object.SetRoomIcon(icon_map.GetSprite(room_controller.room_type));
        map.Add(r, new_map_object);

        foreach (Room.Section section in r.GetSections()) {
            if (section.HasOpenDoorway(Direction.LEFT)) {
                GameObject new_door_object = Instantiate(door_icon);
                new_door_object.transform.SetParent(container.transform, false);
                new_door_object.transform.localPosition = ((Vector2)section.grid_position * scale) + new Vector2(-scale/2, 0);
            }
            if (section.HasOpenDoorway(Direction.RIGHT)) {
                GameObject new_door_object = Instantiate(door_icon);
                new_door_object.transform.SetParent(container.transform, false);
                new_door_object.transform.localPosition = ((Vector2)section.grid_position * scale) + new Vector2(scale / 2, 0);
            }
            if (section.HasOpenDoorway(Direction.BOTTOM)) {
                GameObject new_door_object = Instantiate(door_icon);
                new_door_object.transform.SetParent(container.transform, false);
                new_door_object.transform.localPosition = ((Vector2)section.grid_position * scale) + new Vector2(0, -scale / 2);
            }
            if (section.HasOpenDoorway(Direction.TOP)) {
                GameObject new_door_object = Instantiate(door_icon);
                new_door_object.transform.SetParent(container.transform, false);
                new_door_object.transform.localPosition = ((Vector2)section.grid_position * scale) + new Vector2(0, scale / 2);
            }
        }
    }

    public void FocusRoom(RoomController room_controller) {
        TryAddRoom(room_controller);
        Center(room_controller.room);
    }

    void Center(Room room) {
        if (!map.ContainsKey(room)) return;
        if (current_focus != null) {
            current_focus.SetSprite(false);
        }
        current_focus = map[room];
        container.transform.localPosition = -current_focus.center;
        current_focus.SetSprite(true);
    }

    [System.Serializable]
    class RoomIconMap {
        [SerializeField] Sprite boss, shop, teleporter;

        Dictionary<RoomType, Sprite> room_icons;

        public void Init() {
            room_icons = new Dictionary<RoomType, Sprite>();

            foreach (RoomType t in System.Enum.GetValues(typeof(RoomType))) {
                room_icons.Add(t, null);
            }
            room_icons[RoomType.boss] = boss;
            room_icons[RoomType.shop] = shop;
            room_icons[RoomType.teleporter] = teleporter;
        }

        public Sprite GetSprite(RoomType room_type) {
            return room_icons[room_type];
        }
    }
}