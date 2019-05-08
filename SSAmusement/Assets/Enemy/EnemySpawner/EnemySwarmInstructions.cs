using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[System.Serializable]
public class EnemySwarmInstructions {

    public List<Wave> waves { get { return _waves; } }

    [SerializeField] bool is_random;
    [SerializeField] List<Wave> _waves;

    [SerializeField] int min_waves, max_waves;
    [SerializeField] List<PossibleSpawns> possible_spawns;

    public void Randomize(RNG rng) {
        if (!is_random) {
            return;
        }
        _waves = new List<Wave>();

        int number_of_waves = rng.GetInt(min_waves, max_waves + 1);
        for (int i = 0; i < number_of_waves; i++) {
            waves.Add(new Wave());
            waves[i].spawns = new List<Spawn>();
            foreach (PossibleSpawns ps in possible_spawns) {
                waves[i].spawns.Add(new Spawn(ps.spawn_point, ps.possible_enemies.GetRandom(rng)));
            }
        }
    }

    [System.Serializable]
    public class Wave {
        public List<Spawn> spawns;
    }

    [System.Serializable]
    public class Spawn {
        public EnemySpawnPoint spawn_point;
        public Enemy enemy;

        public Spawn(EnemySpawnPoint sp, Enemy e) {
            spawn_point = sp;
            enemy = e;
        }
    }

    [System.Serializable]
    public class PossibleSpawns {
        public EnemySpawnPoint spawn_point;
        public List<Enemy> possible_enemies;
    }
}
