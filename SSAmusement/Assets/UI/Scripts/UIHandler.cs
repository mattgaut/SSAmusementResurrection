using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    static UIHandler instance;

    [SerializeField] GameObject pause_screen, pause_screen_active_selection;
    [SerializeField] Button unpause_button;
    [SerializeField] GameObject info_screen;
    [SerializeField] GameObject mini_map_object;
    [SerializeField] Map big_mini_map, small_mini_map;
    [SerializeField] Map info_map;
    [SerializeField] InfoDisplay info_display;

    [SerializeField] Image game_over_screen, quick_fade;
    [SerializeField] Text game_over_text;
    [SerializeField] GameObject game_over_active_selection, game_over_panel;
    [SerializeField] Camera death_camera;

    [SerializeField] Image crawl_bg, crawl_fade;
    [SerializeField] Text crawl_text;
    [SerializeField] float fade_length, hold_length;

    public void TogglePauseScreen(bool is_paused) {
        if (is_paused) OnPause();
        else OnUnPause();
    }

    public void ToggleShowInfoScreen(bool is_screen_up) {
        if (is_screen_up) {
            OpenInfo();
        } else {
            CloseInfo();
        }
    }

    public static Coroutine StartEndCrawl() {
        return instance ? instance.LocalStartEndCrawl() : null;
    }

    public Coroutine LocalStartEndCrawl() {
        return StartCoroutine(EndCrawl());
    }
    
    public static void FocusRoom(RoomController room_controller) {
        if (instance == null) return;
        instance.big_mini_map.FocusRoom(room_controller);
        instance.small_mini_map.FocusRoom(room_controller);
        instance.info_map.FocusRoom(room_controller);
    }

    public static void DisplayAchievement(Achievement achievement) {
        if (instance == null) return;
        instance.info_display.Display(achievement.achievement_name, achievement.description, achievement.icon, 2f, Color.white);
    }

    public void DisplayItem(Item i) {
        instance.info_display.Display(i.item_name, i.item_description, i.icon, 2f, Color.white);
    }

    public static IEnumerator FadeToBlack(float fade_in_timer) {
        yield return instance.FadeOut(fade_in_timer);
    }
    public static IEnumerator FadeInFromBlack(float fade_in_timer) {
        yield return instance.FadeIn(fade_in_timer);
    }

    public void StartGameOverCutscene() {
        mini_map_object.SetActive(false);
        StartCoroutine(GameOverRoutine());
    }

    private void Awake() {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    private void Update() {
        if (MyInput.GetButtonDown("AdjustMiniMap")) {
            small_mini_map.gameObject.SetActive(!small_mini_map.gameObject.activeSelf);
            big_mini_map.gameObject.SetActive(!big_mini_map.gameObject.activeSelf);
        }
    }

    private void Start() {
        GameManager.instance.AddOnGameOverEvent(StartGameOverCutscene);
        GameManager.instance.AddOnPauseEvent(TogglePauseScreen);
        GameManager.instance.AddOnSelectEvent(ToggleShowInfoScreen);

        GameManager.instance.player.inventory.on_collect_item += DisplayItem;

        small_mini_map.gameObject.SetActive(false);

        unpause_button.onClick.AddListener(GameManager.instance.TogglePause);
    }

    private void OnDestroy() {
        if (GameManager.has_instance) {
            GameManager.instance.RemoveOnGameOverEvent(StartGameOverCutscene);
            GameManager.instance.RemoveOnPauseEvent(TogglePauseScreen);
            GameManager.instance.RemoveOnSelectEvent(ToggleShowInfoScreen);

            Player player = GameManager.instance.player;
            if (player) {
                player.inventory.on_collect_item -= DisplayItem;
            }
        }
    }

    IEnumerator FadeOut(float fade_in_timer) {
        float timer = 0;
        game_over_screen.enabled = true;
        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            game_over_screen.color = new Color(0, 0, 0, timer / fade_in_timer);
            yield return null;
        }
        game_over_screen.color = new Color(0, 0, 0, 1);
    }

    IEnumerator FadeIn(float fade_in_timer) {
        float timer = 0;
        game_over_screen.enabled = true;
        game_over_screen.color = new Color(0, 0, 0, 1);
        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            game_over_screen.color = new Color(0, 0, 0, 1 - (timer / fade_in_timer));
            yield return null;
        }
        game_over_screen.color = new Color(0, 0, 0, 0);
        game_over_screen.enabled = false;
    }

    IEnumerator GameOverRoutine() {
        death_camera.transform.position = Camera.main.transform.position;
        death_camera.gameObject.SetActive(true);
        float fade_in_timer = 0.5f, timer = 0;


        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            death_camera.transform.position = Camera.main.transform.position;
            quick_fade.color = new Color(0, 0, 0, timer / fade_in_timer);
            yield return null;
        }
        death_camera.transform.position = Camera.main.transform.position;

        fade_in_timer = 2f;
        while (fade_in_timer > 0) {
            fade_in_timer -= Time.deltaTime;
            yield return null;
        }

        fade_in_timer = 2f;
        game_over_screen.enabled = true;
        game_over_text.enabled = true;
        while (timer < fade_in_timer) {
            timer += Time.deltaTime;
            game_over_screen.color = new Color(0, 0, 0, timer / fade_in_timer);
            game_over_text.color = new Color(1, 1, 1, Mathf.Pow(timer / fade_in_timer, 5f));
            yield return null;
        }

        fade_in_timer = 0.5f;
        while (fade_in_timer > 0) {
            fade_in_timer -= Time.deltaTime;
            yield return null;
        }

        game_over_panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(game_over_active_selection);
    }

    IEnumerator EndCrawl() {
        // Fade in
        crawl_fade.enabled = true;
        crawl_text.enabled = true;
        crawl_bg.enabled = true;
        crawl_bg.color = Color.clear;
        crawl_fade.color = Color.clear;
        float timer = fade_length;
        while (timer > 0) {
            timer -= Time.deltaTime;
            crawl_bg.color = new Color(0, 0, 0, 1f - timer / fade_length);
            crawl_text.color = Color.white - Color.black * (timer / fade_length);
            yield return null;
        }
        crawl_bg.color = Color.black;
        // Hold
        timer = 0;
        while (timer < hold_length) {
            timer += Time.deltaTime;
            yield return null;
        }
        // Fade Out
        timer = 0;
        while (timer < fade_length) {
            timer += Time.deltaTime;
            crawl_fade.color = new Color(0, 0, 0, timer / fade_length);
            yield return null;
        }
        crawl_fade.color = Color.black;
        SceneManager.LoadScene("TitleScene");
    }

    void OnPause() {
        pause_screen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pause_screen_active_selection);
    }

    void OnUnPause() {
        pause_screen.SetActive(false);
    }

    void OpenInfo() {
        mini_map_object.SetActive(false);
        info_screen.SetActive(true);
    }

    void CloseInfo() {
        mini_map_object.SetActive(true);
        info_screen.SetActive(false);
    }
}
