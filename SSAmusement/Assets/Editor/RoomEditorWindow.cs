using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class RoomEditorWindow : EditorWindow {

    enum BlockEditMode { Place, Delete, Doorways }
    enum RoomEditMode { Info, Block, RoomSet }
    enum RoomSetEditMode { Enemies, EnemyHierarchy, Objects, ObjectHierarchy }

    string[] block_edit_modes = new string[] { "Place", "Delete", "Doorways" };
    string[] tile_strings = new string[] { "Square", "Upward", "Downward", "Platform" };
    string[] room_edit_modes = new string[] { "Info", "Block", "Room Set" };
    string[] room_set_edit_modes = new string[] { "Place Enemies", "Enemy Hierarchy", "Place Objects", "Object Hierachy"  };

    Texture2D[] tile_icons;
    Texture2D[] enemy_icons;
    Texture2D[] prop_icons;

    BlockEditMode block_mode;
    RoomEditMode room_mode;
    RoomSetEditMode room_set_mode;

    Vector2 scroll_position;
    int object_selection_number = -1;

    Tool last_tool;

    int size_x, size_y;

    RoomEditor room_editor;

    int mouse_held = -1;
    Vector2 mouse_hold_origin;

    KeyCode button_held = KeyCode.None;
    TileType tile_type = TileType.Square;

    LevelSet level_set;

    GameObject mouse_object, prefab_asset_to_spawn;

    List<Enemy> placed_enemies;

    List<RoomProp> placed_props;
    List<RoomProp> available_props;

    string room_name, room_set_name;

    bool mouse_over_scene = false;
    bool room_edited_since_save = true;

    Room room_to_load;
    RoomSet room_set_to_load;

    public static void ShowWindow(RoomEditor room_editor) {
        (GetWindow(typeof(RoomEditorWindow)) as RoomEditorWindow).SetRoomEditor(room_editor);
    }

    public void SetRoomEditor(RoomEditor room_editor) {
        this.room_editor = room_editor;
        size_x = room_editor.edited_room.size.x;
        size_y = room_editor.edited_room.size.y;
        SetLevelSet(room_editor.current_level_set);

        ReloadRoomSet();
    }

    private void OnEnable() {
        last_tool = Tools.current;
        Tools.current = Tool.None;

        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        placed_enemies = new List<Enemy>();
        placed_props = new List<RoomProp>();

        tile_icons = new Texture2D[] {
            Resources.Load("EditorIcons/SquareIcon") as Texture2D,
            Resources.Load("EditorIcons/UpSlopeIcon") as Texture2D,
            Resources.Load("EditorIcons/DownSlopeIcon") as Texture2D,
            Resources.Load("EditorIcons/PlatformIcon") as Texture2D
        };

        available_props = new List<RoomProp>(Resources.LoadAll<RoomProp>("RoomProps"));
        prop_icons = new Texture2D[available_props.Count];
        
        for (int i = 0; i < prop_icons.Length; i++) {
            prop_icons[i] = GetSpriteTexture(available_props[i].sprite);
        }
    }

    private void OnDisable() {
        Tools.current = last_tool;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;

        if (mouse_object != null) DestroyImmediate(mouse_object);
    }

    private void OnSceneGUI(SceneView scene_view) {
        if (room_mode == RoomEditMode.Block && block_mode == BlockEditMode.Doorways) {


            for (int x = 0; x < size_x; x++) {
                for (int y = 0; y < size_y; y++) {
                    Vector2 section_anchor = new Vector2(x * Room.Section.width, y * Room.Section.height);



                    if (x == 0) {
                        if (Handles.Button(section_anchor + (Vector2.up * (Room.Section.height - 1f) / 2f), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)) {
                            room_editor.ToggleDoorwayCanOpen(x, y, Direction.LEFT);
                            room_edited_since_save = true;
                        }

                    }
                    if (x == size_x - 1) {
                        if (Handles.Button(section_anchor + (Vector2.up * (Room.Section.height - 1f) / 2f) + (Vector2.right * (Room.Section.width - 1)), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)) {
                            room_editor.ToggleDoorwayCanOpen(x, y, Direction.RIGHT);
                            room_edited_since_save = true;
                        }
                    }
                    if (y == 0) {
                        if (Handles.Button(section_anchor + (Vector2.right * (Room.Section.width - 1f) / 2f), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)) {
                            room_editor.ToggleDoorwayCanOpen(x, y, Direction.BOTTOM);
                            room_edited_since_save = true;
                        }
                    }
                    if (y == size_y - 1) {
                        if (Handles.Button(section_anchor + (Vector2.right * (Room.Section.width - 1f) / 2f) + (Vector2.up * (Room.Section.height - 1)), Quaternion.identity, 0.5f, 0.5f, Handles.RectangleHandleCap)) {
                            room_editor.ToggleDoorwayCanOpen(x, y, Direction.TOP);
                            room_edited_since_save = true;
                        }
                    }
                }
            }
        }

        HandleInputEvents();



        if (mouse_held == 0) {            
            Vector2 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

            if (room_mode == RoomEditMode.Block) {
                if (mouse_over_scene) FocusWindowIfItsOpen(typeof(SceneView));

                if (block_mode == BlockEditMode.Place && button_held != KeyCode.LeftControl) {
                    if (room_editor.AddTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y))) {
                        room_edited_since_save = true;
                    }                    
                }
                if (block_mode == BlockEditMode.Delete || (block_mode == BlockEditMode.Place && button_held == KeyCode.LeftControl)) {
                    if (room_editor.DeleteTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y))) {
                        room_edited_since_save = true;
                    }
                }
            }
        }

        if (room_mode == RoomEditMode.RoomSet) {
            if (mouse_over_scene) FocusWindowIfItsOpen(typeof(SceneView));

            if (room_set_mode == RoomSetEditMode.Enemies || room_set_mode == RoomSetEditMode.Objects) {
                Vector2 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

                if (mouse_object) {
                    if (button_held == KeyCode.LeftControl) {
                        pos.y = Mathf.FloorToInt(pos.y + 0.5f) - 0.5f;
                    }
                    mouse_object.transform.position = pos;

                    mouse_object.SetActive(mouse_over_scene);

                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F) {
                        if (room_set_mode == RoomSetEditMode.Objects) {
                            mouse_object.transform.rotation *= Quaternion.Euler(0,180,0);

                            Event.current.Use();
                        }
                    }

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                        GameObject new_object = PrefabUtility.InstantiatePrefab(prefab_asset_to_spawn) as GameObject;
                        new_object.transform.SetPositionAndRotation(mouse_object.transform.position, mouse_object.transform.rotation);
                        new_object.name = mouse_object.name;
                        if (room_set_mode == RoomSetEditMode.Enemies) {
                            Enemy new_enemy = new_object.GetComponent<Enemy>();
                            placed_enemies.Add(new_enemy);
                            room_editor.edited_room_set.AddEnemy(new_enemy);
                            new_object.transform.SetParent(room_editor.edited_room_set.transform.Find("Enemies"));
                        } else {
                            placed_props.Add(new_object.GetComponent<RoomProp>());
                            new_object.transform.SetParent(room_editor.edited_room_set.transform.Find("Props"));
                        }
                    }
                }
            }
        } else {
            if (mouse_object)
                mouse_object.SetActive(false);
        }
    }


    void HandleInputEvents() {
        mouse_over_scene = mouseOverWindow == SceneView.currentDrawingSceneView;
        if (Event.current.type == EventType.KeyDown && button_held == KeyCode.None) {
            button_held = Event.current.keyCode;

            GUIUtility.hotControl = 0;
        }
        if (Event.current.type == EventType.KeyUp && button_held == Event.current.keyCode) {
            button_held = KeyCode.None;
        }

        if (Event.current.type == EventType.MouseDown) {
            mouse_held = Event.current.button;

            if (mouse_over_scene) mouse_hold_origin = Event.current.mousePosition;
            else mouse_hold_origin = Vector2.zero;

            GUIUtility.hotControl = 0;
        }
        if (Event.current.type == EventType.MouseUp && Event.current.button == mouse_held) {
            mouse_held = -1;
        }
    }

    void OnGUI() {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Current RoomEditor:", room_editor, typeof(RoomEditor), false);
        EditorGUI.EndDisabledGroup();

        RoomModeToggle();

        if (room_mode == RoomEditMode.Info) {
            LevelSet old_level_set = level_set;
            level_set = (LevelSet)EditorGUILayout.ObjectField("Level Set", level_set, typeof(LevelSet), true);
            if (level_set != old_level_set && level_set != null) {
                SetLevelSet(level_set);
            }

            SetSizeButton();

            SaveRoomOptions();

            LoadRoomOptions();
        }

        if (room_mode == RoomEditMode.Block) {
            BlockModeToggle();

            if (block_mode == BlockEditMode.Place) {
                TileTypeToolbar();
            }
        }

        if (room_mode == RoomEditMode.RoomSet) {
            RoomSetModeToggle();

            if (room_set_mode == RoomSetEditMode.Enemies) {
                EnemySelection();
            } else if (room_set_mode == RoomSetEditMode.EnemyHierarchy) {
                Enemy to_remove = null;
                foreach (Enemy e in placed_enemies) {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        to_remove = e;
                    }
                    if (GUILayout.Button(e.name, EditorStyles.label)) {
                        Selection.objects = new Object[] { e.gameObject };
                        Tools.current = Tool.Move;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (to_remove != null) {
                    placed_enemies.Remove(to_remove);
                    room_editor.RemoveEnemy(to_remove);
                    DestroyImmediate(to_remove.gameObject);
                }
            } else if (room_set_mode == RoomSetEditMode.Objects) {
                ObjectSelection();
            } else if (room_set_mode == RoomSetEditMode.ObjectHierarchy) {
                RoomProp to_remove = null;
                foreach (RoomProp prop in placed_props) {
                    if (prop == null) {
                        continue;
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        to_remove = prop;
                    }
                    if (GUILayout.Button(prop.name, EditorStyles.label)) {
                        Selection.objects = new Object[] { prop.gameObject };
                        Tools.current = Tool.Move;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (to_remove != null) {
                    placed_props.Remove(to_remove);
                    DestroyImmediate(to_remove.gameObject);
                }
            }
        }
    }

    void SetLevelSet(LevelSet level_set) {
        this.level_set = level_set;
        room_editor.SetLevelSet(level_set);

        List<Enemy> available_enemies = level_set.available_enemies;

        enemy_icons = new Texture2D[available_enemies.Count];
        for (int i = 0; i < available_enemies.Count; i++) {
            enemy_icons[i] = available_enemies[i].icon.texture;
        }
    }

    void EnemySelection() {
        scroll_position = GUILayout.BeginScrollView(scroll_position);

        int icon_size = 100;
        float view_width = EditorGUIUtility.currentViewWidth;
        int number_can_fit = (int)view_width / icon_size;
        number_can_fit = System.Math.Min(System.Math.Max(1, number_can_fit), enemy_icons.Length);

        int old_enemy = object_selection_number;
        object_selection_number = GUILayout.SelectionGrid(object_selection_number, enemy_icons, number_can_fit, GUILayout.Height(icon_size * Mathf.CeilToInt((float)enemy_icons.Length / number_can_fit)), GUILayout.MinWidth(icon_size * number_can_fit));

        if (object_selection_number != old_enemy) {
            Enemy to_copy = level_set.available_enemies[object_selection_number];
            SetMouseHoverObject(to_copy.gameObject);
            mouse_object.SetActive(false);
            mouse_object.name = to_copy.name;
        }

        GUILayout.EndScrollView();
    }

    void ObjectSelection() {
        scroll_position = GUILayout.BeginScrollView(scroll_position);

        int icon_size = 100;
        float view_width = EditorGUIUtility.currentViewWidth;
        int number_can_fit = (int)view_width / icon_size;
        number_can_fit = System.Math.Min(System.Math.Max(1, number_can_fit), prop_icons.Length);

        int old_enemy = object_selection_number;
        object_selection_number = GUILayout.SelectionGrid(object_selection_number, prop_icons, number_can_fit, GUILayout.Height(icon_size * Mathf.CeilToInt((float)prop_icons.Length / number_can_fit)), GUILayout.MinWidth(icon_size * number_can_fit));

        if (object_selection_number != old_enemy) {
            RoomProp to_copy = available_props[object_selection_number];
            SetMouseHoverObject(to_copy.gameObject);
            mouse_object.SetActive(false);
            mouse_object.name = to_copy.name;
        }

        GUILayout.EndScrollView();
    }

    void SetMouseHoverObject(GameObject to_copy) {
        if (mouse_object != null) {
            DestroyImmediate(mouse_object);
        }
        mouse_object = PrefabUtility.InstantiatePrefab(to_copy) as GameObject;
        prefab_asset_to_spawn = to_copy;
    }

    void SaveRoomOptions() {
        EditorGUILayout.Space();

        room_name = EditorGUILayout.TextField("Room Name", room_name);

        if (GUILayout.Button("Save Room")) {
            TrySaveRoom();
        }

        EditorGUILayout.Space();

        room_set_name = EditorGUILayout.TextField("Room Set Name", room_set_name);
        
        if (room_edited_since_save || !AssetDatabase.IsValidFolder("Assets/Rooms/" + room_name)) {
            EditorGUI.BeginDisabledGroup(true);
            if (GUILayout.Button("Save Room Set")) {
                TrySaveRoomSet();
            }
            EditorGUI.EndDisabledGroup();
        } else {
            if (GUILayout.Button("Save Room Set")) {
                TrySaveRoomSet();
            }
        }
    }

    void LoadRoomOptions() {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        room_to_load = (Room)EditorGUILayout.ObjectField(room_to_load, typeof(Room), false);

        if (room_to_load == null) EditorGUI.BeginDisabledGroup(true);
        if (GUILayout.Button("Load Room", GUILayout.MaxWidth(160))) {
            room_editor.CopyRoom(room_to_load);
            room_name = room_to_load.name;
            room_edited_since_save = false;
            room_to_load = null;
        }
        if (room_to_load == null) EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        room_set_to_load = (RoomSet)EditorGUILayout.ObjectField(room_set_to_load, typeof(RoomSet), false);

        if (room_set_to_load == null) EditorGUI.BeginDisabledGroup(true);
        if (GUILayout.Button("Load RoomSet", GUILayout.MaxWidth(160))) {
            
            room_editor.CopyRoomSet(room_set_to_load);
            room_set_name = room_set_to_load.name;
            room_set_to_load = null;

            ReloadRoomSet();
        }
        if (room_set_to_load == null) EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();
    }
    void SetSizeButton() {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Width", GUILayout.Width(40));
        size_x = EditorGUILayout.IntField(size_x, GUILayout.Width(20));
        size_x = Mathf.RoundToInt(GUILayout.HorizontalSlider(size_x, 1, 5));
        size_x = System.Math.Max(System.Math.Min(size_x, 5), 0);
        GUILayout.Label("Height", GUILayout.Width(40));
        size_y = EditorGUILayout.IntField(size_y, GUILayout.Width(20));
        size_y = Mathf.RoundToInt(GUILayout.HorizontalSlider(size_y, 1, 5));
        size_y = System.Math.Max(System.Math.Min(size_y, 5), 0);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Set Size")) {
            room_editor.ChangeSize(size_x, size_y);
            room_edited_since_save = true;
        }
    }

    void RoomModeToggle() {
        RoomEditMode old_mode = room_mode;
        room_mode = (RoomEditMode)GUILayout.Toolbar((int)room_mode, room_edit_modes);

        if (room_mode != old_mode) {
            Tools.current = Tool.None;
            SceneView.RepaintAll();
        }
    }

    void BlockModeToggle() {
        BlockEditMode old_mode = block_mode;
        block_mode = (BlockEditMode)GUILayout.Toolbar((int)block_mode, block_edit_modes);

        if (block_mode != old_mode) {
            Tools.current = Tool.None;
            SceneView.RepaintAll();
        }
    }

    void RoomSetModeToggle() {
        RoomSetEditMode old_mode = room_set_mode;

        room_set_mode = (RoomSetEditMode)GUILayout.Toolbar((int)room_set_mode, room_set_edit_modes);

        if (room_set_mode != old_mode) {
            Tools.current = Tool.None;
            object_selection_number = -1;
            SceneView.RepaintAll();
        }
    }

    void TileTypeToolbar() {
        tile_type = (TileType)GUILayout.Toolbar((int)tile_type, tile_icons, GUILayout.MinHeight(40));
        room_editor.SetTileType(tile_type);
    }

    void TrySaveRoom() {
        if (AssetDatabase.IsValidFolder("Assets/Rooms/" + room_name)) {
            if (EditorUtility.DisplayDialog("Are you sure?",
                    "The room already exists. Do you want to overwrite it?",
                    "Yes",
                    "No")) {
                if (EditorUtility.DisplayDialog("Keep Room Sets?",
                    "Delete Room Sets from overwritten Room?",
                    "Yes",
                    "No")) {
                    ClearOldSave();
                    SaveRoom();
                } else {
                    SaveRoom(false);
                }
            }
        } else {
            SaveRoom();
        }
    }

    void TrySaveRoomSet() {
        if (AssetDatabase.AssetPathToGUID("Assets/Rooms/" + room_name + "/RoomSets/" + room_set_name + ".prefab") != "") {
            if (EditorUtility.DisplayDialog("Are you sure?",
                    "The room set already exists. Do you want to overwrite it?",
                    "Yes",
                    "No")) {
                SaveRoomSet();
            }
        } else {
            SaveRoomSet();
        }
    }

    void ClearOldSave() {
        FileUtil.DeleteFileOrDirectory("Assets/Rooms/" + room_name);
        AssetDatabase.Refresh();
    }

    void SaveRoom(bool create_folders = true) {
        if (create_folders) AssetDatabase.CreateFolder("Assets/Rooms", room_name);

        bool successful = false;
        PrefabUtility.SaveAsPrefabAsset(room_editor.edited_room.gameObject, "Assets/Rooms/" + room_name + "/" + room_name + ".prefab", out successful);
        Debug.Log(successful ? "Room saved successfully." : "Error: Room not saved.");

        if (create_folders) AssetDatabase.CreateFolder("Assets/Rooms/" + room_name, "RoomSets");

        if (successful) {
            room_edited_since_save = false;
        }
    }

    void SaveRoomSet() {
        bool successful = false;

        if (AssetDatabase.IsValidFolder("Assets/Rooms/" + room_name + "/RoomSets")) {
            PrefabUtility.SaveAsPrefabAsset(room_editor.edited_room_set.gameObject, "Assets/Rooms/" + room_name + "/RoomSets/" + room_set_name + ".prefab", out successful);
        }
        Debug.Log(successful ? "Room set saved successfully." : "Error: Room set not saved.");
    }

    Texture2D GetSpriteTexture(Sprite sprite) {

        Rect sprite_rect = sprite.textureRect;
        Texture2D new_texture = new Texture2D((int)sprite_rect.width, (int)sprite_rect.height);

        new_texture.SetPixels(sprite.texture.GetPixels((int)sprite_rect.x, (int)sprite_rect.y, new_texture.width, new_texture.height));

        if (new_texture.height < 100) {
            new_texture = ScaleUpTexture(new_texture);
        }

        new_texture.Apply();

        return new_texture;
    }

    Texture2D ScaleUpTexture(Texture2D texture) {
        Color[] colors = texture.GetPixels();

        texture.Resize(texture.width * 2, texture.height * 2);

        texture.Apply();

        Color[] new_colors = new Color[colors.Length * 4];

        for (int i = 0; i < colors.Length; i++) {
            int new_i = i * 2;
            new_i += (new_i / texture.width) * texture.width; 
            new_colors[new_i] = colors[i];
            new_colors[new_i + 1] = colors[i];
            new_colors[new_i + texture.width] = colors[i];
            new_colors[new_i + 1 + texture.width] = colors[i];
        }

        texture.SetPixels(new_colors);

        texture.Apply();

        return texture;
    }

    void ReloadRoomSet() {
        placed_enemies = new List<Enemy>();
        placed_props = new List<RoomProp>();

        if (room_editor.edited_room_set != null) {
            foreach (RoomProp room_prop in room_editor.edited_room_set.GetComponentsInChildren<RoomProp>()) {
                placed_props.Add(room_prop);
            }
            foreach (Enemy enemy in room_editor.edited_room_set.GetEnemies()) {
                placed_enemies.Add(enemy);
            }
        }
    }

    private void Update() {
        if (room_editor == null) {
            Close();
        }
    }
}