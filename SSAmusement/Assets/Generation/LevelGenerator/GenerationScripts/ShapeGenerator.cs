using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : LevelGenerator {

    List<Vector2Int> shape_blocks;
    [SerializeField] [Range(0, 1)] float fill;
    [SerializeField] [Range(3, 20)] int blocks_x, blocks_y;

    protected override void Generate(Level level, RNG rng) {
        GenerateShapes();
        available_spaces = new HashSet<Vector2Int>(shape_blocks);
        InsertRoom(level.spawn_room, Vector2Int.zero);

        foreach (Level.WeightedRoomGroup wrg in level.weighted_groups) {
            List<RoomController> possible_rooms = new List<RoomController>(wrg.rooms);
            int room_count = wrg.GetNumberToSpawn(rng);
            while (possible_rooms.Count > 0 && room_count > 0) {

                int cont_index = rng.GetInt(0, possible_rooms.Count);
                RoomController cont = possible_rooms[cont_index];
                possible_rooms.RemoveAt(cont_index);

                List<Vector2Int> possible_spaces = new List<Vector2Int>(available_spaces);
                while (possible_spaces.Count > 0) {

                    int position_index = rng.GetInt(0, possible_spaces.Count);
                    Vector2Int position = possible_spaces[position_index];
                    possible_spaces.RemoveAt(position_index);

                    if (IslandCanFit(cont.room, position)) {
                        InsertIsland(cont, position);
                        room_count--;
                        if (room_count <= 0) {
                            break;
                        }
                    }
                }
            }
        }

        if (level.boss_rooms.Count > 0 && level.teleporter_rooms.Count > 0) {
            bool teleporter_room_placed = false;
            RoomController teleporter_room_controller = level.teleporter_rooms.GetRandom(rng);
            List<Vector2Int> possible_spaces = new List<Vector2Int>(available_spaces);
            while (possible_spaces.Count > 0) {

                int position_index = rng.GetInt(0, possible_spaces.Count);
                Vector2Int position = possible_spaces[position_index];
                possible_spaces.RemoveAt(position_index);

                if (IslandCanFit(teleporter_room_controller.room, position)) {
                    InsertIsland(teleporter_room_controller, position);
                    teleporter_room_placed = true;
                    break;
                }
            }
            if (!teleporter_room_placed) {
                Debug.LogError("No Suitable position for Teleporter Room");
            }

            RoomController boss_room_controller = level.boss_rooms.GetRandom(rng);
            InsertRoom(boss_room_controller, boss_room_controller.room.size * -1);
        }

        while ((float)available_spaces.Count / shape_blocks.Count > (1 - fill)) {
            Vector2Int next_space;
            RoomController cont;
            List<RoomController> remaining_rooms = new List<RoomController>(level.unweighted_rooms);
            Vector2Int offset = Vector2Int.zero;
            if (remaining_rooms.Count == 0) {
                return;
            }
            bool fit = false;
            do {
                cont = remaining_rooms[rng.GetInt(0, remaining_rooms.Count)];
                remaining_rooms.Remove(cont);
                List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
                do {
                    next_space = possible_spaces[rng.GetInt(0, possible_spaces.Count)];
                    possible_spaces.Remove(next_space);
                    foreach (Vector2Int i in cont.room.GetLocalCoordinatesList().Shuffle(rng)) {
                        if (available_spaces.Contains(next_space + i) && RoomCanFit(cont.room, next_space + i)) {
                            fit = true;
                            offset = i;
                            break;
                        }
                    }

                } while (!fit && possible_spaces.Count > 0);
            } while (!fit && remaining_rooms.Count > 0);
            if (!fit) {
                break;
            } else
                InsertRoom(cont, next_space + offset);
        }
    }

    protected override void HandleIslands(HashSet<Island> islands, RNG rng) {
        foreach (Island island in islands) {
            List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
            possible_spaces.Shuffle(rng);

            bool place_found = false;
            while (possible_spaces.Count > 0 && !place_found) {
                int index = rng.GetInt(0, possible_spaces.Count);
                Vector2Int pos = possible_spaces[index];
                possible_spaces.RemoveAt(index);
                if (RoomCanFit(island.room, pos)) {
                    InsertRoom(island.cont, pos);
                    place_found = true;
                }
            }

            if (!place_found) Debug.LogError("Failed to find place for island: " + island.cont);
        }
    }

    void GenerateShapes() {
        shape_blocks = new List<Vector2Int>();

        for (int i = blocks_x; i >= 0; i--) {
            for (int j = blocks_y; j >= 0; j--) {
                shape_blocks.Add(new Vector2Int(i, j));
            }
        }
    }
}
