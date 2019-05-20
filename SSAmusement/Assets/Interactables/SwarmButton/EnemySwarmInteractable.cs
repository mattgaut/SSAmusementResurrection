using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmInteractable : MonoBehaviour, IInteractable {

    public bool is_available { get { return !used; } }

    [SerializeField] EnemySwarmInstructions instruction;
    [SerializeField] PickupChest reward_chest; 

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
        if (reward_chest != null) {
            RNG rng = RNGSingleton.instance.loot_rng;
            reward_chest.SetSpawnPickups(LootTablesSingleton.instance.regular_chest_loot.GetPilePickupes(rng));
        }
        home = room;
    }

    IEnumerator StartSwarm() {
        if (home) home.SetAllDoorwaysOpen(false);

        for (int i = 0; i < instruction.waves.Count; i++) {
            List<Character> enemies = new List<Character>();
            foreach (EnemySwarmInstructions.Spawn s in instruction.waves[i].spawns) {
                Enemy e = s.spawn_point.SpawnEnemy(s.enemy);
                enemies.Add(e);
                e.on_death += ((killed, killer) => enemies.Remove(killed));
            }

            while (enemies.Count > 0) {
                yield return null;
            }
        }

        if (home) home.SetAllDoorwaysOpen(true);

        if (reward_chest) reward_chest.Open();
    }
}
