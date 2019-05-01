using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : RoomController {

    [SerializeField] Vector2Int entrance_block_coord, fighting_block_coord, leaving_block_coord;
    [SerializeField] Enemy boss;
    [SerializeField] Door door_in, door_to_next_floor, door_lock_in;
    [SerializeField] ItemChest _reward;
    [SerializeField] AudioClip boss_theme;
    [SerializeField] Collider2D boss_blocker;

    [SerializeField] RoomSet room_set;

    bool fighting, boss_dead;

    public ItemChest reward { get { return _reward; } }

    public override RoomType room_type {
        get { return RoomType.boss; }
    }

    public override RoomSet GetRoomsetToLoad() {
        return room_set;
    }

    public void OnEnterArena() {
        FindObjectOfType<CameraFollow>().LerpToFollow(0.25f);

        door_in.Close();
        if (door_to_next_floor) door_to_next_floor.Close();

        fighting = true;

        if (boss != null) {
            boss.GetComponent<EnemyDisplay>().Enable(true);
            boss.SetHome(this);
            boss.GetComponent<EnemyHandler>().SetActive(true);
        }


        door_in.SetHardLocked(true);
        door_lock_in.SetHardLocked(true);
        if (door_to_next_floor) door_to_next_floor.SetHardLocked(true);
        if (boss_theme != null) SoundManager.PlaySong(boss_theme);

        if (boss == null) {
            OnBossDefeated();
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

    /// <summary>
    /// Removes enemy calls OnBossDefeated if it was the rooms boss
    /// </summary>
    /// <param name="enemy"></param>
    public override void RemoveEnemy(Enemy enemy) {
        base.RemoveEnemy(enemy);
        if (enemy == boss) {
            OnBossDefeated();
        }
    }

    public void OnBossDefeated() {
        fighting = false;
        boss_dead = true;

        FindObjectOfType<CameraFollow>().LerpToFollow(1f);

        if (door_to_next_floor) {
            door_to_next_floor.SetHardLocked(false);
            door_to_next_floor.Open();
        }

        if (boss_blocker) boss_blocker.enabled = false;
    }
}
