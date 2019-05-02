using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    [SerializeField] TileSet ts;
    [SerializeField] float room_width, room_height;
    [SerializeField] [Range(0, 1)] float mobility;
    Dictionary<Vector2Int, Room> positions_to_rooms;
    Dictionary<Room.Section, List<Room.Section>> possible_neighbors;
    Dictionary<Room, List<Room>> adjacent_rooms;
    List<Room> rooms;
    BossRoomController boss_room_controller;
    TeleporterRoomController teleporter_room_controller;

    private void Awake() {
        positions_to_rooms = new Dictionary<Vector2Int, Room>();
        adjacent_rooms = new Dictionary<Room, List<Room>>();
        possible_neighbors = new Dictionary<Room.Section, List<Room.Section>>();

        rooms = new List<Room>();
    }

    public void Generate(Dictionary<Vector2Int, Room> room_dict) {
        SpawnRooms(room_dict);
        foreach (Vector2Int v in positions_to_rooms.Keys) {
            FindPossibleNeighbors(v);
        }
        foreach (Room.Section s in possible_neighbors.Keys) {
            foreach (Room.Section n in possible_neighbors[s]) {
                if (n.grid_position.y == s.grid_position.y - 1) {
                    s.SetDoorwayOpen(Direction.BOTTOM);
                    adjacent_rooms[s.room].Add(n.room);
                } else if (n.grid_position.y == s.grid_position.y + 1) {
                    s.SetDoorwayOpen(Direction.TOP);
                    adjacent_rooms[s.room].Add(n.room);
                } else if (n.grid_position.x == s.grid_position.x - 1) {
                    s.SetDoorwayOpen(Direction.LEFT);
                    adjacent_rooms[s.room].Add(n.room);
                } else if (n.grid_position.x == s.grid_position.x + 1) {
                    s.SetDoorwayOpen(Direction.RIGHT);
                    adjacent_rooms[s.room].Add(n.room);
                }
            }
        }
    }

    void SpawnRooms(Dictionary<Vector2Int, Room> room_dict) {
        positions_to_rooms = new Dictionary<Vector2Int, Room>();
        foreach (Vector2Int v in room_dict.Keys) {
            Room new_room = Instantiate(room_dict[v], new Vector3(v.x * room_width, v.y * room_height, 0), Quaternion.identity);
            RoomController new_room_controller = new_room.GetComponent<RoomController>();
            new_room.position = v;
            adjacent_rooms.Add(new_room, new List<Room>());
            foreach (Vector2Int pos in new_room.GetLocalCoordinatesList()) {
                positions_to_rooms.Add(pos + v, new_room);
                foreach (Direction d in System.Enum.GetValues(typeof(Direction))) {
                    if (new_room.HasOpenableDoorway(pos, d)) {
                        new_room.SetDoorwayOpen(pos, d, false);
                    }
                }
            }
            new_room.LoadTileSet(ts);
            new_room_controller.Init();
            if (new_room_controller.room_type == RoomType.boss) {
                boss_room_controller = new_room_controller as BossRoomController;
            } else if (new_room_controller.room_type == RoomType.teleporter) {
                teleporter_room_controller = new_room_controller as TeleporterRoomController;
            } else {
                rooms.Add(new_room);
            }
        }

        List<Room> can_spawn_item = new List<Room>(rooms);
        int target_items = 3;
        while (can_spawn_item.Count > 0 && target_items > 0) {
            can_spawn_item.Shuffle();
            ItemSpawner item_spawn = can_spawn_item[0].GetComponentInChildren<ItemSpawner>();
            if (item_spawn == null) {
                can_spawn_item.RemoveAt(0);
            } else {
                item_spawn.SpawnItemChest().SetSpawnItem(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.item_rng));
                target_items--;
                can_spawn_item.RemoveAt(0);
            }
        }

        if (boss_room_controller != null) {
            if (boss_room_controller != null) {
                boss_room_controller.reward.SetSpawnItem(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.item_rng));
            }
            if (teleporter_room_controller != null) {
                boss_room_controller.teleporter.Link(teleporter_room_controller.teleporter);
            }
        }
        foreach (Room r in rooms) {
            foreach (ItemSpawner ispawn in r.GetComponentsInChildren<ItemSpawner>()) {
                Destroy(ispawn.gameObject);
            }
        }
    }

    void FindPossibleNeighbors(Vector2Int v) {
        Room.Section section = GetSection(v);
        possible_neighbors.Add(section, new List<Room.Section>());
        if (section.HasOpenableDoorway(Direction.BOTTOM) && positions_to_rooms.ContainsKey(v + Vector2Int.down)) {
            Room.Section neighbor = GetSection(v + Vector2Int.down);
            if (neighbor.HasOpenableDoorway(Direction.TOP)) {
                possible_neighbors[section].Add(neighbor);
            }
        }
        if (section.HasOpenableDoorway(Direction.TOP) && positions_to_rooms.ContainsKey(v + Vector2Int.up)) {
            Room.Section neighbor = GetSection(v + Vector2Int.up);
            if (neighbor.HasOpenableDoorway(Direction.BOTTOM)) {
                possible_neighbors[section].Add(neighbor);
            }
        }
        if (section.HasOpenableDoorway(Direction.LEFT) && positions_to_rooms.ContainsKey(v + Vector2Int.left)) {
            Room.Section neighbor = GetSection(v + Vector2Int.left);
            if (neighbor.HasOpenableDoorway(Direction.RIGHT)) {
                possible_neighbors[section].Add(neighbor);
            }
        }
        if (section.HasOpenableDoorway(Direction.RIGHT) && positions_to_rooms.ContainsKey(v + Vector2Int.right)) {
            Room.Section neighbor = GetSection(v + Vector2Int.right);
            if (neighbor.HasOpenableDoorway(Direction.LEFT)) {
                possible_neighbors[section].Add(neighbor);
            }
        }
    }

    Room.Section GetSection(Vector2Int v) {
        return positions_to_rooms[v].GetSection(v - positions_to_rooms[v].position);
    }

    public Dictionary<Room, List<Room>> GetNeighbors() {
        return adjacent_rooms;
    }

    public Room GetOrigin() {
        return positions_to_rooms[Vector2Int.zero];
    }
}
