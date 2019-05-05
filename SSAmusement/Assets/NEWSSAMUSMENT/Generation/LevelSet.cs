using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSet", menuName = "ScriptableObjects/LevelSetObject", order = 1)]
public class LevelSet : ScriptableObject {

    [SerializeField] TileSet _tile_set;
    [SerializeField] Sprite _background;

    public TileSet tile_set { get { return _tile_set; } }
    public Sprite background { get { return _background; } }
}
