using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class Dedicated to controlling the game logic that occurs in relation to rooms
/// </summary>
[RequireComponent(typeof(Room))]
public abstract class RoomController : MonoBehaviour {

    public abstract RoomType room_type { get; }

    public Room room { get; protected set; }

    protected RoomSet loaded_room_set;

    Dictionary<Enemy, Vector3> enemies;

    public virtual void Init() {
        LoadRoomSet(GetRoomsetToLoad());
    }

    public virtual void Activate() {
        foreach (Enemy enemy in enemies.Keys) {
            enemy.GetComponent<EnemyHandler>().SetActive(true);
        }
        room.OnActivate();
    }

    public virtual void Deactivate() {
        foreach (Enemy enemy in enemies.Keys) {
            enemy.GetComponent<EnemyHandler>().SetActive(false);
            enemy.transform.position = enemies[enemy];
        }
        room.OnDeactivate();
    }

    /// <summary>
    /// Clamps position to area defined by the centers of each room section
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Clamped position in bounds</returns>
    public virtual Vector3 ClampToBounds(Vector3 position) {
        Vector3 offset = -new Vector3(0.5f, 0.5f, 0) + new Vector3(Room.Section.width / 2f, Room.Section.height / 2f);
        Vector3 local_position = position - room.transform.position - offset;
        if (local_position.x < 0.5f) {
            local_position.x = 0.5f;
        } else if (local_position.x > ((room.size.x - 1) * Room.Section.width) + 0.5f) {
            local_position.x = ((room.size.x - 1) * Room.Section.width) + 0.5f;
        }
        if (local_position.y < 0.5f) {
            local_position.y = 0.5f;
        } else if (local_position.y >= ((room.size.y - 1) * Room.Section.height) + 0.5f) {
            local_position.y = ((room.size.y - 1) * Room.Section.height) + 0.5f;
        }
        return local_position + room.transform.position + offset;
    }

    public virtual void RemoveEnemy(Enemy enemy) {
        if (enemies.ContainsKey(enemy)) {
            enemies.Remove(enemy);
        }
    }

    protected abstract RoomSet GetRoomsetToLoad();

    /// <summary>
    /// Loads a roomset, takes note of all enemies in room
    /// and gives them loot based on loot tables
    /// </summary>
    protected virtual void LoadRoomSet(RoomSet room_set) {
        if (room_set == null) {
            return;
        }
        if (loaded_room_set != null) {
            Destroy(loaded_room_set.gameObject);
        }
        loaded_room_set = room_set;

        enemies = new Dictionary<Enemy, Vector3>();

        Vector3 offset = new Vector3(1, 1) + (new Vector3(Room.Section.width, Room.Section.height) / 2f);

        loaded_room_set.transform.SetParent(transform);
        loaded_room_set.transform.position = transform.position;

        List<PickupChance> pickups_table = LootTablesSingleton.instance.GetPickupsTable();

        foreach (Enemy e in loaded_room_set.GetEnemies()) {
            enemies.Add(e, e.transform.position);
            e.SetHome(this);
            foreach (PickupChance p in pickups_table) {
                if (RNGSingleton.instance.loot_rng.GetFloat() < p.chance_per_roll) {
                    e.AddDropOnDeath(p.loot);
                }
            }
        }
    }

    private void Awake() {
        enemies = new Dictionary<Enemy, Vector3>();
        room = GetComponent<Room>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBoundBox")) {
            if (collision.gameObject.transform.parent.position.y >= (transform.position.y + 1)) {
                RoomManager.instance.SetActiveRoom(this);
            }
        }
    }
}