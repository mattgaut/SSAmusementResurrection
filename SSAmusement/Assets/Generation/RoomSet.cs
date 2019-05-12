using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSet : MonoBehaviour {
    [SerializeField] List<Enemy> enemies;

    public void Clear() {
        enemies = new List<Enemy>();
    }

    public void AddEnemies(List<Enemy> enemies) {
        this.enemies.AddRange(enemies);
    }

    public void AddEnemy(Enemy enemy) {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy) {
        enemies.Remove(enemy);
    }

    public List<Enemy> GetEnemies() {
        return new List<Enemy>(enemies);
    }
}
