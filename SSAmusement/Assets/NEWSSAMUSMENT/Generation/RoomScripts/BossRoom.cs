using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : Room {

    [SerializeField] Vector2Int entrance_block_coord, fighting_block_coord, leaving_block_coord;

    [SerializeField] Enemy boss;

    [SerializeField] Door door_in, door_to_next_floor, door_lock_in;

    [SerializeField] ItemChest _reward;

    [SerializeField] AudioClip boss_theme;

    [SerializeField] Collider2D boss_blocker;
}
