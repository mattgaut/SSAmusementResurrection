using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

    [SerializeField] LevelGenerator lg;
    [SerializeField] RoomSpawner rs;
    [SerializeField] RoomManager rm;
    [SerializeField] List<Color> colors;
    [SerializeField] GameObject spawn;
    [SerializeField] Transform initial_player_position;
    [SerializeField] AudioClip level_theme;

    public Vector3 spawn_point { get { return initial_player_position.position; } }
    List<GameObject> spawned_object;
    [SerializeField] Player _player;
    public Player player {
        get; private set;
    }
    private void Awake() {
        if (_player == null) {
            player = FindObjectOfType<Player>();
        } else {
            player = _player;
        }
        SoundManager.PlaySong(level_theme);
    }

    // Use this for initialization
    void Start() {
        spawned_object = new List<GameObject>();
        Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
        Clear();
        rooms = lg.Generate();
        rs.Generate(rooms);
        rm.SetRooms(rs.GetNeighbors());
        rm.SetActiveRoom(rs.GetOrigin());
    }

    IEnumerator WaitOneFrame() {
        yield return null;
    }

    // Update is called once per frame
    void CreateBlocks(Dictionary<Vector2Int, Room> rooms) {
        int i = 0;
        foreach (Vector2Int origin in rooms.Keys) {
            foreach (Vector2 v in rooms[origin].GetLocalCoordinatesList()) {
                spawned_object.Add(Instantiate(spawn, new Vector3(v.x, v.y, 0) + new Vector3(origin.x, origin.y), Quaternion.identity));
                spawned_object[spawned_object.Count - 1].GetComponent<SpriteRenderer>().color = colors[i % colors.Count];
            }
            i++;
        }
    }
    void Clear() {
        foreach (GameObject g in spawned_object) {
            Destroy(g);
        }
        spawned_object = new List<GameObject>();
    }
}
