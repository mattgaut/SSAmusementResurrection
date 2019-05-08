using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmInteractable : MonoBehaviour, IInteractable {

    [SerializeField] EnemySwarmInstructions instruction;

    bool used;
    Room home;

    public void Interact(Player player) {
        if (!used) {
            StartCoroutine(StartSwarm());
            used = true;
        }
    }

    public void SetHighlight(bool is_highlighted) {
        
    }

    public void Init(Room room) {
        instruction.Randomize(RNGSingleton.instance.swarm_rng);
        home = room;
    }

    IEnumerator StartSwarm() {
        if (home) home.SetAllDoorwaysOpen(false);

        for (int i = 0; i < instruction.waves.Count; i++) {
            List<Character> enemies = new List<Character>();
            foreach (EnemySwarmInstructions.Spawn s in instruction.waves[i].spawns) {
                Enemy e = s.spawn_point.SpawnEnemy(s.enemy);
                enemies.Add(e);
                e.AddOnDeath((killed, killer) => enemies.Remove(killed));
            }

            while (enemies.Count > 0) {
                yield return null;
            }
        }

        if (home) home.SetAllDoorwaysOpen(true);
    }
}
