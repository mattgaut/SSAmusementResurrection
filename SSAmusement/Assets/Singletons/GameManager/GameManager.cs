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
    public int level_count {
        get; private set;
    }

    public float game_time {
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

    Dictionary<Character.Team, TimeScale> time_scales;
    Dictionary<Character.Team, System.Action<float>> on_time_scale_changed;

    public static float GetDeltaTime(Character.Team? team) {
        if (instance == null || team == null) {
            return Time.deltaTime;
        }
        return instance.time_scales[team.GetValueOrDefault()] * Time.deltaTime;
    }
    public static float GetFixedDeltaTime(Character.Team? team) {
        if (instance == null || team == null) {
            return Time.fixedDeltaTime;
        }
        return instance.time_scales[team.GetValueOrDefault()] * Time.fixedDeltaTime;
    }

    public float GetTeamTimeScale(Character.Team team) {
        return time_scales[team];
    }
    public bool IsTimeFrozen(Character.Team team) {
        return time_scales[team] == 0f;
    }
    public void SetTeamTimeScale(Character.Team team, float new_timescale) {
        if (new_timescale != time_scales[team]) {
            time_scales[team].SetBaseValue(new_timescale);
            on_time_scale_changed[team]?.Invoke(new_timescale);
        }
    }
    public void AddTeamTimeScaleModifier(Character.Team team, float factor) {
        if (factor != 1) {
            time_scales[team].AddModifier(factor);
            on_time_scale_changed[team]?.Invoke(time_scales[team]);
        }
    }
    public void RemoveTeamTimeScaleModifier(Character.Team team, float factor) {
        if (factor != 1) {
            time_scales[team].RemoveModifier(factor);
            on_time_scale_changed[team]?.Invoke(time_scales[team]);
        }
    }

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

    public void AddOnTimeScaleChangedEvent(Character.Team team, System.Action<float> callback) {
        on_time_scale_changed[team] += callback;
    }
    public void RemoveOnTimeScaleChangedEvent(Character.Team team, System.Action<float> callback) {
        on_time_scale_changed[team] -= callback;
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

    public IEnumerator TeamWaitForSeconds(Character.Team? team, float length) {
        float timer = length;
        while (timer > 0) {
            yield return new WaitForFixedUpdate();
            timer -= GetFixedDeltaTime(team);
        }
    }

    protected override void OnAwake() {
        base.OnAwake();
        time_scales = new Dictionary<Character.Team, TimeScale>();
        on_time_scale_changed = new Dictionary<Character.Team, System.Action<float>>();
        foreach (Character.Team team in System.Enum.GetValues(typeof(Character.Team))) {
            time_scales.Add(team, new TimeScale(1f));
            on_time_scale_changed.Add(team, null);
        }
        ResetMemory();
    }

    private void Start() {
        //SceneManager.UnloadSceneAsync("Singletons");
        if (start_game_on_start) StartGame(_player);
    }

    IEnumerator LoadLevelRoutine(Level level) {
        float last_game_time = game_time;
        SceneManager.LoadScene("LevelScene", LoadSceneMode.Single);
        current_level = level;

        yield return null;
        level_count++;
        var rooms = level_generator.GenerateLevel(level, RNGSingleton.instance.room_gen_rng);
        room_spawner.Generate(rooms, level.level_set.tile_set);
        room_manager.LoadBackgrounds(level.level_set.background);
        room_manager.SetRooms(room_spawner.GetNeighbors());
        room_manager.SetActiveRoom(room_spawner.GetOrigin().GetComponent<RoomController>());
        ResetTimeScales();

        if (player != null) player.transform.position = new Vector3(2, 1, 0);
        game_time = last_game_time;
    }

    void Update() {
        if (MyInput.GetButtonDown("Pause")) {
            TogglePause();
        }
        if (MyInput.GetButtonDown("Select") && !is_paused) {
            ToggleShowInfoScreen();
        }
        if (!is_paused && !is_select_screen_up) {
            game_time += GetDeltaTime(Character.Team.enemy);
        }
    }

    void SpawnPlayer(Player prefab) {
        player = Instantiate(prefab);
    }

    void ResetMemory() {
        ResetTimeScales();
        level_count = 0;
        input_locks = 0;
        game_time = 0;
        is_paused = is_select_screen_up = is_cutscene_running = false;
    }

    void ResetTimeScales() {
        Time.timeScale = 1f;
        foreach (Character.Team team in System.Enum.GetValues(typeof(Character.Team))) {
            SetTeamTimeScale(team, 1f);
        }
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

    class TimeScale {
        float base_value;
        List<float> modifiers;

        float last_calculated;
        bool changed = true;
        protected float value {
            get {
                if (changed) {
                    last_calculated = GetModifiedValue();
                    changed = false;
                    return last_calculated;
                } else {
                    return last_calculated;
                }
            }
        }

        public TimeScale(float base_value) {
            this.base_value = base_value;
            modifiers = new List<float>();
        }

        float GetModifiedValue() {
            float to_ret = base_value;
            foreach (float modifier in modifiers) {
                to_ret *= modifier;
            }
            return to_ret;
        }

        public void AddModifier(float modifier) {
            modifiers.Add(modifier);
            changed = true;
        }

        public void RemoveModifier(float modifier) {
            modifiers.Remove(modifier);
            changed = true;
        }

        public void SetBaseValue(float f) {
            base_value = f;
            changed = true;
        }

        public static implicit operator float(TimeScale s) {
            return s.value;
        }
        
    }
}
