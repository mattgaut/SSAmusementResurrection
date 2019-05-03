using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public bool input_active {
        get { return input_locks == 0; }
    }

    bool is_paused, is_select_screen_up, is_cutscene_running;
    int input_locks;

    [SerializeField] UnityEvent on_game_over;
    [SerializeField] UnityEventBool on_select, on_pause;

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

    public void GameOver() {
        on_game_over.Invoke();
        input_locks += 1;
    }

    public void StartCutscene() {
        input_locks += 1;
        is_cutscene_running = true;
    }

    public void EndCutscene() {
        input_locks -= 1;
        is_cutscene_running = false;
    }

    public void DestroyPlayer() {
        Player p = FindObjectOfType<Player>();
        Destroy(p.gameObject);
    }

    public void LoadNextLevel(int level_id) {

    }

    public void LoadScene(string scene, LoadSceneMode mode) {
        ResetMemory();

        DestroyPlayer();

        SceneManager.LoadScene(scene, mode);
    }

    public void LoadScene() {

    }

    protected override void OnAwake() {
        base.OnAwake();
    }

    private void Start() {
        SceneManager.UnloadSceneAsync("Singletons");
    }

    public void TogglePause() {
        is_paused = !is_paused;
        on_pause.Invoke(is_paused);
        if (is_paused) {
            Pause();
        } else {
            UnPause();
        }
    }

    void Update() {
        if (Input.GetButtonDown("Pause")) {
            TogglePause();
        }
        if (Input.GetButtonDown("Select") && !is_paused) {
            ToggleShowInfoScreen();
        }
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
