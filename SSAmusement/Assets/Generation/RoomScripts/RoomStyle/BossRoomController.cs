using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : RoomController {

    [SerializeField] Vector2Int entrance_block_coord, fighting_block_coord, leaving_block_coord;
    [SerializeField] List<Enemy> bosses;
    [SerializeField] Door door_to_next_floor;
    [SerializeField] ItemChest _reward;
    [SerializeField] AudioClip boss_theme;
    [SerializeField] Collider2D boss_blocker;

    [SerializeField] RoomSet room_set;

    [SerializeField] Teleporter _teleporter;

    bool fighting, boss_dead;

    public ItemChest reward { get { return _reward; } }

    public Teleporter teleporter {
        get { return _teleporter; }
    }

    public override RoomType room_type {
        get { return RoomType.boss; }
    }

    /// <summary>
    /// Triggers the beginning of the boss fight
    /// </summary>
    public void OnEnterArena() {
        if (boss_dead) return;

        FindObjectOfType<CameraFollow>().LerpToFollow(0.25f);

        teleporter.SetOpen(false);
        door_to_next_floor?.Close();

        fighting = true;

        door_to_next_floor?.SetHardLocked(true);
        if (boss_theme != null) SoundManager.PlaySong(boss_theme);

        if (bosses != null) {
            foreach (Enemy boss in bosses) {
                boss.GetComponent<EnemyDisplay>().Enable(true);
                boss.SetHome(this);
                boss.GetComponent<EnemyHandler>().SetActive(true);
            }
        } else {
            OnBossDefeated();
        }
    }

    /// <summary>
    /// Removes enemy calls OnBossDefeated if it was the rooms boss
    /// </summary>
    /// <param name="enemy"></param>
    public override void RemoveEnemy(Enemy enemy) {
        base.RemoveEnemy(enemy);
        if (bosses.Contains(enemy)) {
            bosses.Remove(enemy);
            if (bosses.Count == 0) OnBossDefeated();
        }
    }

    /// <summary>
    /// Clamps position to bounds dependant on what stage of boss fight the room is in
    /// Pre, During, or Post, Boss fight
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public override Vector3 ClampToBounds(Vector3 position) {
        Vector3 offset = -new Vector3(0.5f, 0.5f, 0) + new Vector3(Room.Section.width / 2f, Room.Section.height / 2f);
        Vector3 local_position = position - transform.position - offset;
        if (!fighting && !boss_dead) {
            return ClampToEntranceAndFightingBlock(local_position) + transform.position + offset;
        } else if (!fighting && boss_dead) {
            return ClampToFightingAndExitBlock(local_position) + transform.position + offset;
        } else {
            return ClampToFightingBlock(local_position) + transform.position + offset;
        }
    }

    protected override RoomSet GetRoomsetToLoad() {
        return room_set;
    }

    protected void OnBossDefeated() {
        fighting = false;
        boss_dead = true;

        FindObjectOfType<CameraFollow>().LerpToFollow(1f);

        if (door_to_next_floor) {
            door_to_next_floor.SetHardLocked(false);
            door_to_next_floor.Open();
        }

        teleporter.arrival_event.RemoveListener(() => OnEnterArena());
        teleporter.SetOpen(true);

        if (boss_blocker) boss_blocker.enabled = false;
    }

    private void Awake() {
        teleporter.arrival_event.AddListener(() => OnEnterArena());
    }

    Vector3 ClampToEntranceBlock(Vector3 position) {
        position.x = (entrance_block_coord.x * Room.Section.width) + 0.5f;
        position.y = (entrance_block_coord.y * Room.Section.height) + 0.5f;

        return position;
    }

    Vector3 ClampToEntranceAndFightingBlock(Vector3 position) {
        float top_bound = (Mathf.Max(fighting_block_coord.y, entrance_block_coord.y) * Room.Section.height) + 0.5f;
        float bottom_bound = (Mathf.Min(fighting_block_coord.y, entrance_block_coord.y) * Room.Section.height) + 0.5f;
        float left_bound = (Mathf.Min(fighting_block_coord.x, entrance_block_coord.x) * Room.Section.width) + 0.5f;
        float right_bound = (Mathf.Max(fighting_block_coord.x, entrance_block_coord.x) * Room.Section.width) + 0.5f;

        position.x = Mathf.Max(Mathf.Min(right_bound, position.x), left_bound);
        position.y = Mathf.Max(Mathf.Min(top_bound, position.y), bottom_bound);

        return position;
    }
    Vector3 ClampToFightingBlock(Vector3 position) {
        position.x = (fighting_block_coord.x * Room.Section.width) + 0.5f;
        position.y = (fighting_block_coord.y * Room.Section.height) + 0.5f;

        return position;
    }
    Vector3 ClampToFightingAndExitBlock(Vector3 position) {
        float top_bound = (Mathf.Max(fighting_block_coord.y, leaving_block_coord.y) * Room.Section.height) + 0.5f;
        float bottom_bound = (Mathf.Min(fighting_block_coord.y, leaving_block_coord.y) * Room.Section.height) + 0.5f;
        float left_bound = (Mathf.Min(fighting_block_coord.x, leaving_block_coord.x) * Room.Section.width) + 0.5f;
        float right_bound = (Mathf.Max(fighting_block_coord.x, leaving_block_coord.x) * Room.Section.width) + 0.5f;

        position.x = Mathf.Max(Mathf.Min(right_bound, position.x), left_bound);
        position.y = Mathf.Max(Mathf.Min(top_bound, position.y), bottom_bound);

        return position;
    }
}
