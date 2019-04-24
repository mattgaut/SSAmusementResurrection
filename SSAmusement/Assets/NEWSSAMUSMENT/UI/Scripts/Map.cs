using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    [SerializeField] MapObject map_object;
    [SerializeField] GameObject door_icon;
    [SerializeField] GameObject container;
    [SerializeField] float scale;

    Dictionary<Room, MapObject> map;
    MapObject current_focus;

    public void Awake() {
        map = new Dictionary<Room, MapObject>();
    }

    public void TryAddRoom(Room r) {
        if (map.ContainsKey(r)) {
            return;
        }
        MapObject new_map_object = Instantiate(map_object);
        new_map_object.transform.SetParent(container.transform, false);
        new_map_object.SetToRoom(r, scale);
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
    public void Center(Room r) {
        if (!map.ContainsKey(r)) return;
        if (current_focus != null) {
            current_focus.SetSprite(false);
        }
        current_focus = map[r];
        container.transform.localPosition = -current_focus.center;
        current_focus.SetSprite(true);
    }
    public void FocusRoom(Room r) {
        TryAddRoom(r);
        Center(r);
    }
}
