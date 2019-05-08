using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    [SerializeField] Transform spawn_point;
    [SerializeField] float spawn_time;

    public Enemy SpawnEnemy(Enemy prefab) {
        Enemy new_enemy = Instantiate(prefab);

        new_enemy.transform.position = spawn_point.position;

        StartCoroutine(SpawnRoutine(new_enemy));

        return new_enemy;
    }

    IEnumerator SpawnRoutine(Enemy new_enemy) {
        SpriteRenderer[] enemy_sprites = new_enemy.GetComponentsInChildren<SpriteRenderer>();

        int lock_value = new_enemy.LockInvincibility();

        float timer = 0;
        while (timer < spawn_time) {
            SetColor(enemy_sprites, (Color.white * Mathf.Pow(timer / spawn_time, 2f)));
            yield return null;
            timer += Time.deltaTime;
        }
        SetColor(enemy_sprites, Color.white);

        new_enemy.GetComponent<EnemyHandler>().SetActive(true);

        new_enemy.UnlockInvincibility(lock_value);
    }

    void SetColor(SpriteRenderer[] sprites, Color color) {
        foreach (SpriteRenderer sprite in sprites) {
            sprite.color = color;
        }
    }
}
