using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    [SerializeField] Transform spawn_point;
    [SerializeField] float spawn_time;

    [SerializeField] Enemy enemy;

    private void Awake() {
        SpawnEnemy(enemy);
    }

    public void SpawnEnemy(Enemy prefab) {
        Enemy new_enemy = Instantiate(prefab);

        new_enemy.transform.position = spawn_point.position;

        StartCoroutine(SpawnRoutine(new_enemy));
    }

    IEnumerator SpawnRoutine(Enemy new_enemy) {
        SpriteRenderer[] enemy_sprites = new_enemy.GetComponentsInChildren<SpriteRenderer>();
        
        float timer = 0;
        while (timer < spawn_time) {
            SetColor(enemy_sprites, (Color.white * Mathf.Pow(timer / spawn_time, 2f)));
            yield return null;
            timer += Time.deltaTime;
        }
        SetColor(enemy_sprites, Color.white);

        new_enemy.GetComponent<EnemyHandler>().SetActive(true);
    }

    void SetColor(SpriteRenderer[] sprites, Color color) {
        foreach (SpriteRenderer sprite in sprites) {
            sprite.color = color;
        }
    }
}
