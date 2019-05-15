using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGenerator : MonoBehaviour {

    protected Dictionary<Vector2Int, Room.Section> tiles;
    protected Dictionary<Vector2Int, RoomController> room_origins;
    protected HashSet<Vector2Int> available_spaces;
    protected HashSet<Vector2Int> adjacent_spaces;

    Dictionary<Vector2Int, Island> islands;

    protected virtual void Clear() {
        tiles = new Dictionary<Vector2Int, Room.Section>();
        islands = new Dictionary<Vector2Int, Island>();
        available_spaces = new HashSet<Vector2Int>();
        room_origins = new Dictionary<Vector2Int, RoomController>();
        adjacent_spaces = new HashSet<Vector2Int>();
    }

    public Dictionary<Vector2Int, RoomController> GenerateLevel(Level level, RNG rng) {
        Clear();

        Generate(level, rng);

        HashSet<Island> unique_islands = new HashSet<Island>();
        foreach (Island i in islands.Values) {
            if (room_origins.Remove(i.origin)) {
                unique_islands.Add(i);
                foreach (Vector2Int pos in i.room.GetLocalCoordinatesList()) {
                    available_spaces.Add(i.origin + pos);
                    if (HasViableNeighbor(i.origin + pos)) {
                        adjacent_spaces.Add(i.origin + pos);
                    }
                }
            }
        }

        HandleIslands(unique_islands, rng);

        return room_origins;
    }

    protected abstract void Generate(Level level, RNG rng);

    protected abstract void HandleIslands(HashSet<Island> islands, RNG rng);

    protected virtual void InsertRoom(RoomController cont, Vector2Int position) {
        room_origins.Add(position, cont);
        foreach (Vector2Int local_position in cont.room.GetLocalCoordinatesList()) {
            Room.Section section = cont.room.GetSection(local_position);
            tiles.Add(position + local_position, section);
            available_spaces.Remove(position + local_position);
            adjacent_spaces.Remove(position + local_position);

            UpdateAdjacentSpaces(section, local_position + position);
        }
    }

    protected virtual void InsertIsland(RoomController cont, Vector2Int position) {
        room_origins.Add(position, cont);
        bool is_true_island = true;
        foreach (Vector2Int local_position in cont.room.GetLocalCoordinatesList()) {
            Room.Section section = cont.room.GetSection(local_position);
            islands.Add(position + local_position, new Island(position, cont));
            available_spaces.Remove(position + local_position);
            adjacent_spaces.Remove(position + local_position);

            Vector2Int pos = local_position + position + Vector2Int.down;
            if (section.HasOpenableDoorway(Direction.BOTTOM) && tiles.ContainsKey(pos) && tiles[pos].HasOpenableDoorway(Direction.TOP)) {
                is_true_island = false;
                break;
            }
            pos = local_position + position + Vector2Int.up;
            if (section.HasOpenableDoorway(Direction.TOP) && tiles.ContainsKey(pos) && tiles[pos].HasOpenableDoorway(Direction.BOTTOM)) {
                is_true_island = false;
                break;
            }
            pos = local_position + position + Vector2Int.left;
            if (section.HasOpenableDoorway(Direction.LEFT) && tiles.ContainsKey(pos) && tiles[pos].HasOpenableDoorway(Direction.RIGHT)) {
                is_true_island = false;
                break;
            }
            pos = local_position + position + Vector2Int.right;
            if (section.HasOpenableDoorway(Direction.RIGHT) && tiles.ContainsKey(pos) && tiles[pos].HasOpenableDoorway(Direction.LEFT)) {
                is_true_island = false;
                break;
            }
        }
        if (!is_true_island) {
            ConvertIsland(position);
        }
    }

    protected virtual void ConvertIsland(Vector2Int position) {
        Room room = islands[position].room;
        position = islands[position].origin;
        foreach (Vector2Int local_position in room.GetLocalCoordinatesList()) {
            Room.Section section = room.GetSection(local_position);
            islands.Remove(position + local_position);
            tiles.Add(position + local_position, section);
            adjacent_spaces.Remove(position + local_position);

            UpdateAdjacentSpaces(section, local_position + position);
        }
    }

    /// <summary>
    /// Checks if room can fit and has something to connect to
    /// </summary>
    /// <param name="room"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected bool RoomCanFit(Room room, Vector2Int position) {
        foreach (Vector2Int local_position in room.GetLocalCoordinatesList()) {
            if (tiles.ContainsKey(position + local_position) || islands.ContainsKey(position + local_position)) {
                return false;
            }
        }
        foreach (Vector2Int local_position in room.GetLocalCoordinatesList()) {
            if (SectionConnects(room.GetSection(local_position), position + local_position)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a room can fit in the position
    /// If room can not freely connect through every doorway may
    /// not be able to be reached
    /// </summary>
    /// <param name="room"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected bool IslandCanFit(Room room, Vector2Int position) {
        foreach (Vector2Int local_position in room.GetLocalCoordinatesList()) {
            if (tiles.ContainsKey(position + local_position) || islands.ContainsKey(position + local_position)) {
                return false;
            }
        }
        return true;
    }

    protected bool SectionConnects(Room.Section section, Vector2Int position) {
        if (section.HasOpenableDoorway(Direction.BOTTOM) && tiles.ContainsKey(position + Vector2Int.down) && tiles[position + Vector2Int.down].HasOpenableDoorway(Direction.TOP)) {
            return true;
        }
        if (section.HasOpenableDoorway(Direction.TOP) && tiles.ContainsKey(position + Vector2Int.up) && tiles[position + Vector2Int.up].HasOpenableDoorway(Direction.BOTTOM)) {
            return true;
        }
        if (section.HasOpenableDoorway(Direction.LEFT) && tiles.ContainsKey(position + Vector2Int.left) && tiles[position + Vector2Int.left].HasOpenableDoorway(Direction.RIGHT)) {
            return true;
        }
        if (section.HasOpenableDoorway(Direction.RIGHT) && tiles.ContainsKey(position + Vector2Int.right) && tiles[position + Vector2Int.right].HasOpenableDoorway(Direction.LEFT)) {
            return true;
        }
        return false;
    }

    void UpdateAdjacentSpaces(Room.Section section, Vector2Int position) {
        Vector2Int pos = position + Vector2Int.down;
        if (section.HasOpenableDoorway(Direction.BOTTOM) && !tiles.ContainsKey(pos)) {
            if (islands.ContainsKey(pos)) {
                ConvertIsland(islands[pos].origin);
            } else {
                adjacent_spaces.Add(pos);
            }
        }
        pos = position + Vector2Int.up;
        if (section.HasOpenableDoorway(Direction.TOP) && !tiles.ContainsKey(pos)) {
            if (islands.ContainsKey(pos)) {
                ConvertIsland(islands[pos].origin);
            } else {
                adjacent_spaces.Add(pos);
            }
        }
        pos = position + Vector2Int.left;
        if (section.HasOpenableDoorway(Direction.LEFT) && !tiles.ContainsKey(pos)) {
            if (islands.ContainsKey(pos)) {
                ConvertIsland(islands[pos].origin);
            } else {
                adjacent_spaces.Add(pos);
            }
        }
        pos = position + Vector2Int.right;
        if (section.HasOpenableDoorway(Direction.RIGHT) && !tiles.ContainsKey(pos)) {
            if (islands.ContainsKey(pos)) {
                ConvertIsland(islands[pos].origin);
            } else {
                adjacent_spaces.Add(pos);
            }
        }
    }

    bool HasViableNeighbor(Vector2Int vector) {
        foreach (var pair in vector.GetNeighborsWithDirection()) {
            if (tiles.ContainsKey(pair.first)) {
                if (tiles[pair.first].HasOpenableDoorway((Direction)(-(int)pair.second))) {
                    return true;
                }
            }
        }
        return false;
    }

    public RoomController GetInitialRoom() {
        return room_origins[Vector2Int.zero];
    }

    protected class Island {
        public Vector2Int origin;
        public RoomController cont;
        public Room room {
            get { return cont.room; }
        }

        public Island(Vector2Int origin, RoomController cont) {
            this.origin = origin;
            this.cont = cont;
        }
    }
}