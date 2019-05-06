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

        int shop_count = level.shop_rooms.GetNumberToSpawn(rng);

        List<RoomController> possible_shops = new List<RoomController>(level.shop_rooms.rooms);
        while (possible_shops.Count > 0 && shop_count > 0) {

            int cont_index = rng.GetInt(0, possible_shops.Count);
            RoomController cont = possible_shops[cont_index];
            possible_shops.RemoveAt(cont_index);

            List<Vector2Int> possible_spaces = new List<Vector2Int>(available_spaces);
            while (possible_spaces.Count > 0) {

                int position_index = rng.GetInt(0, possible_spaces.Count);
                Vector2Int position = possible_spaces[position_index];
                possible_spaces.RemoveAt(position_index);

                if (IslandCanFit(cont.room, position)) {
                    InsertIsland(cont, position);
                    shop_count--;
                    if (shop_count <= 0) {
                        break;
                    }
                }
            }
        }

        while ((float)available_spaces.Count / shape_blocks.Count > (1 - fill)) {
            Vector2Int next_space;
            RoomController cont;
            List<RoomController> remaining_rooms = new List<RoomController>(level.unweighted_rooms);
            Vector2Int offset = Vector2Int.zero;

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

        if (level.boss_rooms.Count > 0 && level.teleporter_rooms.Count > 0) {
            bool teleporter_room_placed = false;
            Vector2Int next_space;
            List<RoomController> remaining_rooms = new List<RoomController>(level.unweighted_rooms);
            Vector2Int offset = Vector2Int.zero;
            RoomController teleporter_room_controller = level.teleporter_rooms.GetRandom(rng);
            List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
            do {
                next_space = possible_spaces[RNGSingleton.instance.room_gen_rng.GetInt(0, possible_spaces.Count)];
                possible_spaces.Remove(next_space);
                foreach (Vector2Int i in teleporter_room_controller.room.GetLocalCoordinatesList().Shuffle(rng)) {
                    if (RoomCanFit(teleporter_room_controller.room, next_space + i)) {
                        teleporter_room_placed = true;
                        offset = i;
                        break;
                    }
                }
            } while (!teleporter_room_placed && possible_spaces.Count > 0);
            if (teleporter_room_placed) {
                InsertRoom(teleporter_room_controller, next_space + offset);
            } else {
                Debug.LogError("No Suitable position for Teleporter Room");
            }

            RoomController boss_room_controller = level.boss_rooms.GetRandom(rng); 
            InsertRoom(boss_room_controller, boss_room_controller.room.size * -1);
        }
    }

    protected override void HandleIslands(HashSet<Island> islands, RNG rng) {
        foreach (Island island in islands) {
            List<Vector2Int> possible_spaces = new List<Vector2Int>(adjacent_spaces);
            possible_spaces.Shuffle(rng);

            while (possible_spaces.Count > 0) {
                Vector2Int pos = possible_spaces[0];
                possible_spaces.RemoveAt(0);
                if (RoomCanFit(island.room, pos)) {
                    InsertRoom(island.cont, pos);
                    break;
                }
            }

            Debug.LogError("Failed to find place for island: " + island.cont);
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
