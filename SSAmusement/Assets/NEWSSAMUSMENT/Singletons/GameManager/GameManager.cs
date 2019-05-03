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

    bool is_paused, is_select_screen_up, is_cutscene_running;
    int input_locks;

    [SerializeField] UnityEvent on_game_over;
    [SerializeField] UnityEventBool on_select, on_pause;

    [SerializeField] bool spawn_on_start;
    [SerializeField] Player _player;

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
        LoadScene("Level1", LoadSceneMode.Single);
        SpawnPlayer(selected_player_prefab);
    }

    /// <summary>
    /// Loads level of the game
    /// </summary>
    /// <param name="level_id">Level id to load</param>
    public void LoadNextLevel(int level_id) {

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

    protected override void OnAwake() {
        base.OnAwake();
        if (spawn_on_start) SpawnPlayer(_player);
    }

    private void Start() {
        SceneManager.UnloadSceneAsync("Singletons");
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
