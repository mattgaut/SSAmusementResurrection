using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { TOP, BOTTOM, LEFT, RIGHT }

public class Room : MonoBehaviour {

    public enum Type { basic, boss, shop }

    [SerializeField] protected Tile[] tiles;

    [SerializeField] protected Tile[] swaptiles;

    [SerializeField] protected Section[] sections;

    [SerializeField] protected Vector2Int _size;

    [SerializeField] protected List<RoomSet> room_sets;

    protected RoomSet loaded_room_set;

    protected Vector2Int _position; // Achored to bottom leftmost block

    BoundaryBox enemy_bound_box;

    public Vector2Int size { get { return _size; } private set { _size = value; } }
    public Vector2Int position { get { return _position; } set { _position = value; } }
    public Vector2 local_center { get { return new Vector2(size.x * Section.width, size.y * Section.height) / 2f; } }
    public virtual Type room_type { get { return Type.basic; } }

    Dictionary<Enemy, Vector3> enemies;

    public virtual void Init() {
        SpawnRandomRoomset();

        List<PickupChance> pickups_table = LootTablesSingleton.instance.GetPickupsTable();
        foreach (Enemy e in enemies.Keys) {
            foreach (PickupChance p in pickups_table) {
                if (RNGSingleton.instance.loot_rng.GetFloat() < p.chance_per_roll) {
                    e.AddDropOnDeath(p.loot);
                }
            }
        }
    }

    public void SetSize(Vector2Int new_size) {
        Section[] new_sections = new Section[new_size.x * new_size.y];

        // Initialize new blocks
        for (int x = 0; x < new_size.x; x++) {
            for (int y = 0; y < new_size.y; y++) {
                if (x > size.x - 1 || y > size.y - 1 || sections.Length <= (x + new_size.x * y)) {
                    new_sections[x + new_size.x * y] = new Section(this, new Vector2Int(x, y));
                } else {
                    new_sections[x + new_size.x * y] = sections[x + size.x * y];
                }
            }
        }

        // Set up new Doorways
        for (int x = 0; x < new_size.x; x++) {
            for (int y = 0; y < new_size.y; y++) {

                if (x == 0) {
                    new_sections[x + new_size.x * y].SetDoorway(new Doorway(), Direction.LEFT);
                } else {
                    new_sections[x + new_size.x * y].SetDoorway(null, Direction.LEFT);
                }

                if (x == new_size.x - 1) {
                    new_sections[x + new_size.x * y].SetDoorway(new Doorway(), Direction.RIGHT);
                } else {
                    new_sections[x + new_size.x * y].SetDoorway(null, Direction.RIGHT);
                }

                if (y == 0) {
                    new_sections[x + new_size.x * y].SetDoorway(new Doorway(), Direction.BOTTOM);
                } else {
                    new_sections[x + new_size.x * y].SetDoorway(null, Direction.BOTTOM);
                }

                if (y == new_size.y - 1) {
                    new_sections[x + new_size.x * y].SetDoorway(new Doorway(), Direction.TOP);
                } else {
                    new_sections[x + new_size.x * y].SetDoorway(null, Direction.TOP);
                }
            }
        }

        sections = new_sections;
        size = new_size;

        // Clear Tiles
        tiles = new Tile[sections.Length * Section.height * Section.width];
        swaptiles = new Tile[sections.Length * Section.width];
    }

    public Section GetSection(Vector2Int pos) {
        if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y) {
            return null;
        }

        return sections[pos.x + size.x * pos.y];
    }
    public List<Section> GetSections() {
        return new List<Section>(sections);
    }

    public List<Tile> GetTiles() {
        return new List<Tile>(tiles);
    }

    public List<Tile> GetSwapTiles() {
        return new List<Tile>(swaptiles);
    }

    public List<Vector2Int> GetLocalCoordinatesList() {
        List<Vector2Int> vectors = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                vectors.Add(new Vector2Int(x, y));
            }
        }

        return vectors;
    }

    public bool HasOpenableDoorway(Vector2Int pos, Direction direction) {
        int id = pos.x + size.x * pos.y;
        if (id < sections.Length && sections[id].HasDoorway(direction)) {
            return sections[id].GetDoorway(direction).can_open;
        }
        return false;
    }

    public void AddTile(Tile tile, int x, int y) {
        if (IsTilePositionInBounds(x, y) && tile != null) {
            int index = x + (size.x * Section.width * y);

            tile.position = new Vector2Int(x, y);
            tiles[index] = tile;


            // Adjust adjacent tile borders
            if (tile.tile_type != TileType.Platform) {
                TileType neighbor_type = TileType.Square;
                if (HasTile(x + 1, y) && x + 1 != size.x * Section.width) {
                    tiles[index + 1].SetLeftBorder(tile.tile_type == TileType.DownwardTriangle);
                    neighbor_type = tiles[index + 1].tile_type;
                    tile.SetRightBorder(neighbor_type == TileType.Platform || neighbor_type == TileType.UpwardTriangle);
                } else {
                    tile.SetRightBorder(true);
                }
                if (HasTile(x - 1, y) && x != 0) {
                    tiles[index - 1].SetRightBorder(tile.tile_type == TileType.UpwardTriangle);
                    neighbor_type = tiles[index - 1].tile_type;
                    tile.SetLeftBorder(neighbor_type == TileType.Platform || neighbor_type == TileType.DownwardTriangle);
                } else {
                    tile.SetLeftBorder(true);
                }
                if (HasTile(x, y + 1) && y + 1 != size.y * Section.height) {
                    tiles[index + (size.x * Section.width)].SetBottomBorder(tile.tile_type == TileType.DownwardTriangle || tile.tile_type == TileType.UpwardTriangle);
                    neighbor_type = tiles[index + (size.x * Section.width)].tile_type;
                    tile.SetTopBorder(neighbor_type == TileType.Platform);
                } else {
                    tile.SetTopBorder(true);
                }
                if (HasTile(x, y - 1) && y != 0) {
                    tiles[index - (size.x * Section.width)].SetTopBorder(false);
                    neighbor_type = tiles[index - (size.x * Section.width)].tile_type;
                    tile.SetBottomBorder(neighbor_type == TileType.Platform || neighbor_type == TileType.UpwardTriangle || neighbor_type == TileType.DownwardTriangle);
                } else {
                    tile.SetBottomBorder(true);
                }
            }
        }
    }

    public void AddSwapTile(int x, Tile tile) {
        if (IsTilePositionInBounds(x, 0)) {
            tile.position = new Vector2Int(x, 0);
            swaptiles[x] = tile;
        }
    }

    public Tile RemoveSwapTile(int x) {
        if (IsTilePositionInBounds(x, 0)) {
            Tile tile = swaptiles[x];
            swaptiles[x] = null;
            return tile;
        }
        return null;
    }

    public bool IsTilePositionInBounds(int x, int y) {
        return (x >= 0 && size.x * Section.width > x && y >= 0 && size.y * Section.height > y);
    }
    public bool IsTilePositionInEditableBounds(int x, int y) {
        return (x >= 1 && (size.x * Section.width) - 1 > x && y >= 1 && (size.y * Section.height) - 1 > y);
    }

    public bool HasTile(int x, int y) {
        return IsTilePositionInBounds(x, y) && tiles[x + (size.x * Section.width * y)] != null;
    }

    public Tile RemoveTile(int x, int y) {
        if (!HasTile(x, y)) {
            return null;
        }
        int index = x + (size.x * Section.width * y);

        Tile tile = tiles[index];
        tiles[index] = null;

        // Adjust adjacent tile borders
        if (tile.tile_type != TileType.Platform) {
            if (HasTile(x + 1, y) && x + 1 != size.x * Section.width) {
                tiles[index + 1].SetLeftBorder(true);
            }
            if (HasTile(x - 1, y) && x != 0) {
                tiles[index - 1].SetRightBorder(true);
            }
            if (HasTile(x, y + 1) && y + 1 != size.y * Section.height) {
                tiles[index + (size.x * Section.width)].SetBottomBorder(true);
            }
            if (HasTile(x, y - 1) && y != 0) {
                tiles[index - (size.x * Section.width)].SetTopBorder(true);
            }
        }
        return tile;
    }

    public void SetSize(int x, int y) {
        SetSize(new Vector2Int(x, y));
    }

    public void ToggleDoorwayCanOpen(Vector2Int position, Direction direction) {
        if (position.x < size.x && position.y < size.y && sections[position.x + size.x * position.y].HasDoorway(direction)) {
            Doorway doorway = sections[position.x + size.x * position.y].GetDoorway(direction);
            if (doorway.is_open) {
                SetDoorwayOpen(position, direction, false);
            }
            doorway.can_open = !doorway.can_open;
        }
    }
    public void SetDoorwayCanOpen(Vector2Int position, Direction direction, bool can_open) {
        if (position.x < size.x && position.y < size.y && sections[position.x + size.x * position.y].HasDoorway(direction)) {
            Doorway doorway = sections[position.x + size.x * position.y].GetDoorway(direction);
            if (doorway.can_open == can_open) {
                return;
            }
            if (doorway.is_open && can_open == false) {
                SetDoorwayOpen(position, direction, false);
            }
            doorway.can_open = can_open;
        }
    }

    public void ToggleDoorwayOpen(Vector2Int position, Direction direction) {
        if (position.x < size.x && position.y < size.y && sections[position.x + size.x * position.y].HasDoorway(direction)) {
            SetDoorwayOpen(position, direction, !sections[position.x + size.x * position.y].GetDoorway(direction).is_open);
        }
    }

    public void SetDoorwayOpen(Vector2Int position, Direction direction, bool is_open) {
        if (position.x < size.x && position.y < size.y && sections[position.x + size.x * position.y].HasDoorway(direction)) {
            if (!sections[position.x + size.x * position.y].GetDoorway(direction).can_open && is_open) {
                Debug.Log("Doorway Should not Open");
                return;
            }
            sections[position.x + size.x * position.y].GetDoorway(direction).is_open = is_open;

            int num_blocks_wide = Section.width * size.x;
            if (direction == Direction.LEFT) {
                tiles[(position.y * Section.height + 4) * num_blocks_wide].gameObject.SetActive(!is_open);
                tiles[(position.y * Section.height + 5) * num_blocks_wide].gameObject.SetActive(!is_open);
                tiles[(position.y * Section.height + 3) * num_blocks_wide].SetTopBorder(is_open);
                tiles[(position.y * Section.height + 6) * num_blocks_wide].SetBottomBorder(is_open);
            } else if (direction == Direction.RIGHT) {
                tiles[(position.y * Section.height + 5) * num_blocks_wide - 1].gameObject.SetActive(!is_open);
                tiles[(position.y * Section.height + 6) * num_blocks_wide - 1].gameObject.SetActive(!is_open);
                tiles[(position.y * Section.height + 4) * num_blocks_wide - 1].SetTopBorder(is_open);
                tiles[(position.y * Section.height + 7) * num_blocks_wide - 1].SetBottomBorder(is_open);
            } else if (direction == Direction.BOTTOM) {
                SwapTile(position.x * Section.width + 7);
                SwapTile(position.x * Section.width + 8);
                SwapTile(position.x * Section.width + 9);
                SwapTile(position.x * Section.width + 10);
                tiles[position.x * Section.width + 6].SetRightBorder(is_open);
                tiles[position.x * Section.width + 11].SetLeftBorder(is_open);
            } else if (direction == Direction.TOP) {
                tiles[position.x * Section.width + 7 + ((size.y * Section.height - 1) * num_blocks_wide)].gameObject.SetActive(!is_open);
                tiles[position.x * Section.width + 8 + ((size.y * Section.height - 1) * num_blocks_wide)].gameObject.SetActive(!is_open);
                tiles[position.x * Section.width + 9 + ((size.y * Section.height - 1) * num_blocks_wide)].gameObject.SetActive(!is_open);
                tiles[position.x * Section.width + 10 + ((size.y * Section.height - 1) * num_blocks_wide)].gameObject.SetActive(!is_open);
                tiles[position.x * Section.width + 6 + ((size.y * Section.height - 1) * num_blocks_wide)].SetRightBorder(is_open);
                tiles[position.x * Section.width + 11 + ((size.y * Section.height - 1) * num_blocks_wide)].SetLeftBorder(is_open);
            }
        }
    }

    public void LoadTileSet(TileSet set) {
        foreach (Tile t in tiles) {
            if (t != null) {
                t.LoadSprite(set);
            }
        }
        foreach (Tile t in swaptiles) {
            if (t != null) {
                t.LoadSprite(set);
            }
        }
    }

    public void AddRoomSet(RoomSet room_set) {
        room_sets.Add(room_set);
    }

    protected virtual void SpawnRandomRoomset() {
        if (loaded_room_set != null && room_sets.Count > 0) {
            Destroy(loaded_room_set.gameObject);
        }

        enemies = new Dictionary<Enemy, Vector3>();

        Vector3 offset = new Vector3(1,1) + (new Vector3(Section.width, Section.height) / 2f);

        if (room_sets.Count > 0) {
            loaded_room_set = Instantiate(room_sets[RNGSingleton.instance.room_gen_rng.GetInt(0, room_sets.Count)]);
            loaded_room_set.transform.SetParent(transform);
            loaded_room_set.transform.position = transform.position;

            foreach (Enemy e in loaded_room_set.GetEnemies()) {
                enemies.Add(e, e.transform.position);
                e.SetRoom(this);
            }
        }
    }

    public virtual Vector3 ClampToCameraBounds(Vector3 position) {
        Vector3 offset = - new Vector3(0.5f, 0.5f, 0) + new Vector3(Section.width / 2f, Section.height / 2f);
        Vector3 local_position = position - transform.position - offset;
        if (local_position.x < 0.5f) {
            local_position.x = 0.5f;
        } else if (local_position.x > ((size.x - 1) * Section.width) + 0.5f) {
            local_position.x = ((size.x - 1) * Section.width) + 0.5f;
        }
        if (local_position.y < 0.5f) {
            local_position.y = 0.5f;
        } else if (local_position.y >= ((size.y - 1) * Section.height) + 0.5f) {
            local_position.y = ((size.y - 1) * Section.height) + 0.5f;
        }
        return local_position + transform.position + offset;
    }

    public virtual void OnActivate() {
        foreach (Enemy enemy in enemies.Keys) {
            enemy.GetComponent<EnemyHandler>().SetActive(true);
        }
        enemy_bound_box.Enable();
    }
    public virtual void OnDeactivate() {
        foreach (Enemy enemy in enemies.Keys) {
            enemy.GetComponent<EnemyHandler>().SetActive(false);
            enemy.transform.position = enemies[enemy];
        }
        enemy_bound_box.Disable();
    }

    public virtual void RemoveEnemy(Enemy enemy) {
        if (enemies.ContainsKey(enemy)) {
            enemies.Remove(enemy);
        }
    }

    protected virtual void Awake() {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.offset = local_center;
        collider.size = new Vector2(size.x * Section.width - 1, size.y * Section.height - 1);

        enemy_bound_box = GetComponentInChildren<BoundaryBox>();
        if (enemy_bound_box != null) {
            enemy_bound_box.Set(this);
            enemy_bound_box.Disable();
        }

        enemies = new Dictionary<Enemy, Vector3>();
    }

    protected virtual void Start() { }

    void SwapTile(int id) {
        if (IsTilePositionInBounds(id, 0)) {

            Tile temp = tiles[id];
            tiles[id] = swaptiles[id];
            swaptiles[id] = temp;

            if (swaptiles[id] != null) {
                swaptiles[id].gameObject.SetActive(false);
            }
            if (tiles[id] != null) {
                tiles[id].gameObject.SetActive(true);
            }
        }
    }

    [System.Serializable]
    public class Section {
        public static int width { get { return 18; } }
        public static int height { get { return 10; } }
        public static int door_width { get { return 4; } }
        public static int door_height { get { return 2; } }



        [SerializeField] Room _room;

        [SerializeField] protected Vector2Int _position; // Relative to Room position
        [SerializeField] protected Doorway _left, _right, _top, _bottom;

        public Room room { get { return _room; } }

        public Vector2Int position { get { return _position; } private set { _position = value; } }
        public Vector2Int grid_position { get { return _position + room.position; } }

        public Doorway left { get { return _left; } private set { _left = value; } }
        public Doorway right { get { return _right; } private set { _right = value; } }
        public Doorway top { get { return _top; } private set { _top = value; } }
        public Doorway bottom { get { return _bottom; } private set { _bottom = value; } }

        public Section(Room room, Vector2Int position) {
            this.position = position;
            _room = room;
        }

        public Doorway GetDoorway(Direction direction) {
            if (direction == Direction.LEFT) {
                return left;
            } else if (direction == Direction.RIGHT) {
                return right;
            } else if (direction == Direction.TOP) {
                return top;
            } else {
                return bottom;
            }
        }

        public void ClearDoorways() {
            left = right = top = bottom = null;
        }

        public void SetDoorway(Doorway doorway, Direction direction) {
            if (direction == Direction.LEFT) {
                left = doorway;
            } else if (direction == Direction.RIGHT) {
                right = doorway;
            } else if (direction == Direction.TOP) {
                top = doorway;
            } else if (direction == Direction.BOTTOM) {
                bottom = doorway;
            }
        }

        public bool HasDoorway(Direction direction) {
            if (direction == Direction.LEFT) {
                return left != null;
            } else if (direction == Direction.RIGHT) {
                return right != null;
            } else if (direction == Direction.TOP) {
                return top != null;
            } else {
                return bottom != null;
            }
        }

        public bool HasOpenDoorway(Direction direction) {
            if (direction == Direction.LEFT) {
                return left != null && left.is_open;
            } else if (direction == Direction.RIGHT) {
                return right != null && right.is_open;
            } else if (direction == Direction.TOP) {
                return top != null && top.is_open;
            } else {
                return bottom != null && bottom.is_open;
            }
        }

        public bool HasOpenableDoorway(Direction direction) {
            if (direction == Direction.LEFT) {
                return left != null && left.can_open;
            } else if (direction == Direction.RIGHT) {
                return right != null && right.can_open;
            } else if (direction == Direction.TOP) {
                return top != null && top.can_open;
            } else {
                return bottom != null && bottom.can_open;
            }
        }

        public void SetDoorwayOpen(Direction direction, bool is_open = true) {
            room.SetDoorwayOpen(position, direction, is_open);
        }
    }

    [System.Serializable]
    public class Doorway {
        [SerializeField] private bool _is_open;
        [SerializeField] private bool _can_open;

        public bool is_open { get { return _is_open; } set { _is_open = value; } }
        public bool can_open { get { return _can_open; } set { _can_open = value; } }

        public Doorway() {
            can_open = false;
            is_open = false;
        }
    }
}