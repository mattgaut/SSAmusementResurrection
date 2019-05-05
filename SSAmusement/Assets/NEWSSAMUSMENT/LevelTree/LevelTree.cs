using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelTree", menuName = "ScriptableObjects/LevelTree", order = 4)]
public class LevelTree : ScriptableObject {

    public Level first_level {
        get { return _first_level; }
    }

    [SerializeField] Level _first_level;
    [SerializeField] List<Node> nodes;
    Dictionary<Level, Node> level_to_node;

    public ICollection<Level> GetNextLevels(Level current_level) {
        if (!level_to_node.ContainsKey(current_level)) {
            return new List<Level>();
        }
        return level_to_node[current_level].possible_next_levels;
    }

    private void OnEnable() {
        level_to_node = new Dictionary<Level, Node>();
        foreach (Node n in nodes) {
            if (n.level !=  null) level_to_node.Add(n.level, n);
        }
    }

    [System.Serializable]
    class Node {
        public Level level;
        public List<Level> possible_next_levels;
    }
}
