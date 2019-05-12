using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 

public class RoomEditor : MonoBehaviour {

    [SerializeField] SpriteRenderer background;
    [SerializeField] Room _edited_room;
    [SerializeField] RoomSet _edited_room_set;

    [SerializeField] Tile square, upward, downward, platform;
    [SerializeField] LevelAesthetics level_set;

    TileType current_tile_type;

    public Room edited_room {
        get { return _edited_room; }
    }
    public RoomSet edited_room_set {
        get { return _edited_room_set; }
    }

    public LevelAesthetics current_level_set { get { return level_set; } }

    public void SetLevelSet(LevelAesthetics level_set) {
        this.level_set = level_set;
        background.sprite = level_set.background;

        edited_room.LoadTileSet(level_set.tile_set);
    }

    public void SetTileType(TileType type) {
        current_tile_type = type;
    }

    public void ChangeSize(Vector2Int size) {
        List<Tile> old_tiles = edited_room.GetTiles();
        old_tiles.AddRange(edited_room.GetSwapTiles());

        foreach (Tile t in old_tiles) {
            if (t != null) {
                if (Application.isPlaying) {
                    Destroy(t.gameObject);
                } else {
                    DestroyImmediate(t.gameObject);
                }
            }
        }

        edited_room.SetSize(size);

        for (int x = 0; x < size.x * Room.Section.width; x++) {
            for (int y = 0; y < size.y * Room.Section.height; y++) {
                if (x == 0 || x == size.x * Room.Section.width - 1 || y == 0 || y == size.y * Room.Section.height - 1) {
                    AddTile(square, x, y);
                }
            }
        }

        for (int x = 0; x < size.x; x++) {
            int door_start_point = Room.Section.width / 2 - Room.Section.door_width / 2;
            for (int y = door_start_point; y < door_start_point + 4; y++) {
                AddSwapTile(x * Room.Section.width + y);
            }
        }
    }

    public void ToggleDoorwayCanOpen(int x, int y, Direction direction) {
        edited_room.ToggleDoorwayCanOpen(new Vector2Int(x, y), direction);
        if (edited_room.GetSection(new Vector2Int(x, y)).GetDoorway(direction).can_open) {
            edited_room.SetDoorwayOpen(new Vector2Int(x, y), direction, true);
        }
    }

    public void SetDoorwayCanOpen(int x, int y, Direction direction, bool open) {
        edited_room.SetDoorwayCanOpen(new Vector2Int(x, y), direction, open);
        if (edited_room.GetSection(new Vector2Int(x, y)).GetDoorway(direction).can_open) {
            edited_room.SetDoorwayOpen(new Vector2Int(x, y), direction, true);
        }
    }

    public bool AddTile(int x, int y) {
        if (!edited_room.IsTilePositionInEditableBounds(x, y) || edited_room.HasTile(x, y)) {
            return false;
        }
        Tile to_spawn = null;
        if (current_tile_type == TileType.Square) {
            to_spawn = square;
        } else if (current_tile_type == TileType.Platform) {
            to_spawn = platform;
        } else if (current_tile_type == TileType.UpwardTriangle) {
            to_spawn = upward;
        } else if (current_tile_type == TileType.DownwardTriangle) {
            to_spawn = downward;
        } else {
            return false;
        }
        AddTile(to_spawn, x, y);
        return true;
    }

    public void AddSwapTile(int x) {
        Tile new_tile = (Tile)PrefabUtility.InstantiatePrefab(platform, edited_room.transform.Find("Tiles"));
        new_tile.transform.position = new Vector3(x, 0);
        edited_room.AddSwapTile(x, new_tile);
        new_tile.LoadSprite(level_set.tile_set);
        new_tile.gameObject.SetActive(false);
    }

    public bool DeleteTile(int x, int y) {
        if (!edited_room.IsTilePositionInEditableBounds(x, y) || !edited_room.HasTile(x, y)) {
            return false;
        }
        DestroyImmediate(edited_room.RemoveTile(x, y).gameObject);
        return true;
    }

    public void ChangeSize(int x, int y) {
        ChangeSize(new Vector2Int(x, y));
    }

    public void AddEnemy(Enemy enemy) {
        edited_room_set.AddEnemy(enemy);
    }

    public void RemoveEnemy(Enemy enemy) {
        edited_room_set.RemoveEnemy(enemy);
    }

    public void ClearRoomSet() {
        for (int i = edited_room_set.transform.childCount - 1; i >= 0; i--) {
            DestroyAllChildren(edited_room_set.transform.GetChild(i));
        }
        edited_room_set.Clear();
    }

    public void CopyRoom(Room to_copy) {
        ChangeSize(to_copy.size);
        foreach (Tile t in to_copy.GetTiles()) {
            if (t != null && edited_room.IsTilePositionInEditableBounds(t.position.x, t.position.y)) {
                Tile original_tile = PrefabUtility.GetCorrespondingObjectFromOriginalSource(t);
                if (original_tile == t) {
                    AddTile(t, t.position.x, t.position.y, false);
                } else {
                    AddTile(original_tile, t.position.x, t.position.y);
                }
            }
        }
        foreach (Room.Section section in to_copy.GetSections()) {
            foreach (Direction d in System.Enum.GetValues(typeof(Direction))) {
                if (section.HasOpenableDoorway(d)) {
                    SetDoorwayCanOpen(section.position.x, section.position.y, d, true);
                }
            }
        }
    }

    public void CopyRoomSet(RoomSet to_copy) {
        ClearRoomSet();

        Transform copy_from_transform = to_copy.transform.Find("Enemies");
        Transform copy_to_transform = edited_room_set.transform.Find("Enemies");
        CopyAllChildren(copy_from_transform, copy_to_transform);

        foreach (Enemy e in copy_to_transform.GetComponentsInChildren<Enemy>()) {
            AddEnemy(e);
        }

        copy_from_transform = to_copy.transform.Find("Props");
        copy_to_transform = edited_room_set.transform.Find("Props");
        CopyAllChildren(copy_from_transform, copy_to_transform);

        copy_from_transform = to_copy.transform.Find("Doors");
        copy_to_transform = edited_room_set.transform.Find("Doors");
        CopyAllChildren(copy_from_transform, copy_to_transform);


    }

    void AddTile(Tile tile, int x, int y, bool spawn_as_prefab = true) {
        Tile new_tile;
        if (spawn_as_prefab) {
            new_tile = (Tile)PrefabUtility.InstantiatePrefab(tile, edited_room.transform.Find("Tiles"));
        } else {
            new_tile = Instantiate(tile, edited_room.transform.Find("Tiles"));
        }

        new_tile.transform.position = new Vector3(x, y);
        edited_room.AddTile(new_tile, x, y);
        new_tile.LoadSprite(level_set.tile_set);
    }

    void CopyAllChildren(Transform from, Transform to) {
        for (int i = 0; i < from.transform.childCount; i++) {
            GameObject object_to_copy = PrefabUtility.GetCorrespondingObjectFromOriginalSource(from.transform.GetChild(i).gameObject);
            GameObject new_object;
            if (object_to_copy != null && object_to_copy != from.transform.GetChild(i).gameObject) {
                new_object = PrefabUtility.InstantiatePrefab(object_to_copy, to) as GameObject;
            } else {
                new_object = Instantiate(from.transform.GetChild(i).gameObject, to);
            }
            new_object.transform.localPosition = from.transform.GetChild(i).gameObject.transform.localPosition;
        }
    }

    void DestroyAllChildren(Transform t) {
        for (int i = t.childCount - 1; i >= 0; i--) {
            if (Application.isPlaying) {
                Destroy(t.GetChild(i).gameObject);
            } else {
                DestroyImmediate(t.GetChild(i).gameObject);
            }
        }
    }
}

#endif