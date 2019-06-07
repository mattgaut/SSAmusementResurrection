using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    TileSet tile_set;
    [SerializeField] float room_width, room_height;
    [SerializeField] [Range(0, 1)] float mobility;
    [SerializeField] BossKeyPickup boss_key;
    Dictionary<Vector2Int, Room> positions_to_rooms;
    Dictionary<Room.Section, List<Room.Section>> possible_neighbors;
    Dictionary<Room, List<Room>> adjacent_rooms;
    List<Room> rooms;
    BossRoomController boss_room_controller;
    TeleporterRoomController teleporter_room_controller;

    [SerializeField] StatBuff health_buff, speed_buff, power_buff, armor_buff;

    public void Generate(Dictionary<Vector2Int, RoomController> room_dict, TileSet ts) {
        Clear();

        tile_set = ts;
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

        foreach (Room.Section s in possible_neighbors.Keys) {
            foreach (Direction d in System.Enum.GetValues(typeof(Direction))) {
                if (s.HasDoorway(d) && !s.GetDoorway(d).is_open) {
                    s.GetDoorway(d).can_open = false;
                }
            }
        }
    }

    public Dictionary<Room, List<Room>> GetNeighbors() {
        return adjacent_rooms;
    }

    public Room GetOrigin() {
        return positions_to_rooms[Vector2Int.zero];
    }

    void SpawnRooms(Dictionary<Vector2Int, RoomController> room_dict) {
        positions_to_rooms = new Dictionary<Vector2Int, Room>();
        List<Enemy> enemies = new List<Enemy>();
        foreach (Vector2Int v in room_dict.Keys) {
            RoomController new_cont = Instantiate(room_dict[v], new Vector3(v.x * room_width, v.y * room_height, 0), Quaternion.identity);
            Room new_room = new_cont.room;
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
            new_room.LoadTileSet(tile_set);
            new_cont.Init();
            enemies.AddRange(new_cont.GetEnemies());
            if (new_cont.room_type == RoomType.boss) {
                boss_room_controller = new_cont as BossRoomController;
            } else if (new_cont.room_type == RoomType.teleporter) {
                teleporter_room_controller = new_cont as TeleporterRoomController;
            } else {
                rooms.Add(new_room);
            }
        }

        int count = GameManager.instance.level_count - 1;
        health_buff.SetFlat(3 *  count);
        health_buff.SetMulti((.4f * count));
        speed_buff.SetMulti((.25f * count));
        armor_buff.SetFlat(1f * count);
        power_buff.SetFlat(2 * count);
        power_buff.SetMulti((.2f * count));

        foreach (Enemy e in enemies) {
            power_buff.GetInstance().Apply(e);
            armor_buff.GetInstance().Apply(e);
            speed_buff.GetInstance().Apply(e);
            health_buff.GetInstance().Apply(e);
        }

        if (boss_room_controller != null) {
            if (boss_room_controller != null) {
                boss_room_controller.reward.SetSpawnItem(ItemListSingleton.instance.GetRandomItem(RNGSingleton.instance.item_rng));
            }
            if (teleporter_room_controller != null) {
                boss_room_controller.teleporter.Link(teleporter_room_controller.teleporter);
            }
            foreach (Enemy e in boss_room_controller.GetEnemies()) {
                enemies.Remove(e);
            }
        }

        if (enemies.Count > 0) {
            enemies[RNGSingleton.instance.room_gen_rng.GetInt(0, enemies.Count)].AddDropOnDeath(boss_key);
        } else {
            if (teleporter_room_controller != null) {
                teleporter_room_controller.teleporter.SetOpen(true);
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

    void Clear() {
        positions_to_rooms = new Dictionary<Vector2Int, Room>();
        adjacent_rooms = new Dictionary<Room, List<Room>>();
        possible_neighbors = new Dictionary<Room.Section, List<Room.Section>>();

        rooms = new List<Room>();

        boss_room_controller = null;
        teleporter_room_controller = null;
        tile_set = null;
    }
}
