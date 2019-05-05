using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public bool input_active {
        get { return input_locks == 0; }
    }

    public Player player {
        get; private set;
    }

    public Level current_level {
        get; private set;
    }

    bool is_paused, is_select_screen_up, is_cutscene_running;
    int input_locks;

    [SerializeField] UnityEvent on_game_over;
    [SerializeField] UnityEventBool on_select, on_pause;

    [SerializeField] bool start_game_on_start;
    [SerializeField] Player _player;

    [SerializeField] LevelTree level_tree;

    [SerializeField] LevelGenerator level_generator;
    [SerializeField] RoomSpawner room_spawner;
    [SerializeField] RoomManager room_manager;

    public void AddOnPauseEvent(UnityAction<bool> action) {
        on_pause.AddListener(action);
    }

    public void AddOnSelectEvent(UnityAction<bool> action) {
        on_select.AddListener(action);
    }

    public void AddOnGameOverEvent(UnityAction action) {
        on_game_over.AddListener(action);
    }

    public void RemoveOnPauseEvent(UnityAction<bool> action) {
        on_pause.RemoveListener(action);
    }

    public void RemoveOnSelectEvent(UnityAction<bool> action) {
        on_select.RemoveListener(action);
    }

    public void RemoveOnGameOverEvent(UnityAction action) {
        on_game_over.RemoveListener(action);
    }

    /// <summary>
    /// Locks player input and invokes on game over event
    /// </summary>
    public void GameOver() {
        on_game_over.Invoke();
        input_locks += 1;
    }

    /// <summary>
    /// Locks player input and raises Cutscene Running flag
    /// </summary>
    public void StartCutscene() {
        input_locks += 1;
        is_cutscene_running = true;
    }

    /// <summary>
    /// Removes player input lock and lowers Cutscene Running flag
    /// </summary>
    public void EndCutscene() {
        input_locks -= 1;
        is_cutscene_running = false;
    }

    /// <summary>
    /// Destroys Currently Loaded Player
    /// </summary>
    public void DestroyPlayer() {
        if (player != null) {
            Destroy(player.gameObject);
            player = null;
        }
    }

    /// <summary>
    /// Destroys player, instantiates new one and loads first level.
    /// </summary>
    /// <param name="selected_player_prefab">Player prefab to instantiate</param>
    public void StartGame(Player selected_player_prefab) {
        DestroyPlayer();
        ResetMemory();
        SpawnPlayer(selected_player_prefab);
        LoadLevel(level_tree.first_level);
        player.transform.position = new Vector3(2, 1, 0);
    }

    /// <summary>
    /// Loads level of the game at random chosen from the list of levels
    /// that can possibly follow current level
    /// </summary>
    /// <param name="level"></param>
    public void LoadNextLevelAtRandom() {
        LoadLevel(level_tree.GetNextLevels(current_level).GetRandom(RNGSingleton.instance.room_gen_rng));
    }

    /// <summary>
    /// Loads level of the game
    /// </summary>
    /// <param name="level"></param>
    public void LoadLevel(Level level) {
        StartCoroutine(LoadLevelRoutine(level));
    }   

    /// <summary>
    /// Destroy the player and loads a given scene.
    /// Use this to navigate to menu scenes.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    public void LoadScene(string scene, LoadSceneMode mode) {
        ResetMemory();

        DestroyPlayer();

        SceneManager.LoadScene(scene, mode);
    }

    /// <summary>
    /// Toggles pause and Invokes on pause event with current pause status
    /// </summary>
    public void TogglePause() {
        is_paused = !is_paused;
        on_pause.Invoke(is_paused);
        if (is_paused) {
            Pause();
        } else {
            UnPause();
        }
    }

    private void Start() {
        //SceneManager.UnloadSceneAsync("Singletons");
        if (start_game_on_start) StartGame(_player);
    }

    IEnumerator LoadLevelRoutine(Level level) {
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        current_level = level;

        yield return null;

        var ret = level_generator.GenerateLevel(level, RNGSingleton.instance.room_gen_rng);
        room_spawner.Generate(ret, RNGSingleton.instance.loot_rng, level.level_set.tile_set);
        room_manager.LoadBackgrounds();
        room_manager.SetRooms(room_spawner.GetNeighbors());
        room_manager.SetActiveRoom(room_spawner.GetOrigin().GetComponent<RoomController>());

        if (player != null) player.transform.position = new Vector3(2, 1, 0);
    }

    void Update() {
        if (Input.GetButtonDown("Pause")) {
            TogglePause();
        }
        if (Input.GetButtonDown("Select") && !is_paused) {
            ToggleShowInfoScreen();
        }
    }

    void SpawnPlayer(Player prefab) {
        player = Instantiate(prefab);
    }

    void ResetMemory() {
        Time.timeScale = 1;
        input_locks = 0;
        is_paused = is_select_screen_up = is_cutscene_running = false;
    }

    void Pause() {
        input_locks += 1;
        Time.timeScale = 0;
    }

    void UnPause() {
        input_locks -= 1;
        Time.timeScale = is_select_screen_up ? 0 : 1;
    }

    void ToggleShowInfoScreen() {
        is_select_screen_up = !is_select_screen_up;
        on_select.Invoke(is_select_screen_up);
        if (is_select_screen_up) {
            OnSelect();
        } else {
            OnUnSelect();
        }
    }

    void OnSelect() {
        input_locks += 1;
        Time.timeScale = 0;
    }

    void OnUnSelect() {
        input_locks -= 1;
        Time.timeScale = is_paused ? 0 : 1;
    }
}
