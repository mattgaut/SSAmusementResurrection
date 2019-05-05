using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGenerator : MonoBehaviour {

    protected Dictionary<Vector2Int, Room.Section> tiles;
    protected Dictionary<Vector2Int, RoomController> room_origins;
    protected HashSet<Vector2Int> available_spaces;
    protected List<Vector2Int> adjacent_spaces;

    protected virtual void Clear() {
        tiles = new Dictionary<Vector2Int, Room.Section>();
        available_spaces = new HashSet<Vector2Int>();
        room_origins = new Dictionary<Vector2Int, RoomController>();
        adjacent_spaces = new List<Vector2Int>();
    }

    public Dictionary<Vector2Int, RoomController> GenerateLevel(Level level, RNG rng) {
        Clear();

        return Generate(level, rng);
    }

    protected abstract Dictionary<Vector2Int, RoomController> Generate(Level level, RNG rng);

    protected virtual void InsertRoom(RoomController cont, Vector2Int position) {
        room_origins.Add(position, cont);
        foreach (Vector2Int local_position in cont.room.GetLocalCoordinatesList()) {
            Room.Section section = cont.room.GetSection(local_position);
            tiles.Add(position + local_position, section);
            if (available_spaces.Contains(position + local_position)) {
                available_spaces.Remove(position + local_position);
            }
            adjacent_spaces.Remove(position);


            Vector2Int pos = local_position + position + Vector2Int.down;
            if (section.HasOpenableDoorway(Direction.BOTTOM) && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            pos = local_position + position + Vector2Int.up;
            if (section.HasOpenableDoorway(Direction.TOP) && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            pos = local_position + position + Vector2Int.left;
            if (section.HasOpenableDoorway(Direction.LEFT) && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }
            pos = local_position + position + Vector2Int.right;
            if (section.HasOpenableDoorway(Direction.RIGHT) && !tiles.ContainsKey(pos) && !adjacent_spaces.Contains(pos)) {
                adjacent_spaces.Add(pos);
            }

        }
    }

    protected bool RoomCanFit(Room r, Vector2Int position) {
        foreach (Vector2Int local_position in r.GetLocalCoordinatesList()) {
            if (tiles.ContainsKey(position + local_position)) {
                return false;
            }
        }
        foreach (Vector2Int local_position in r.GetLocalCoordinatesList()) {
            if (SectionConnects(r.GetSection(local_position), position + local_position)) {
                return true;
            }
        }
        return false;
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

    public RoomController GetInitialRoom() {
        return room_origins[Vector2Int.zero];
    }
}