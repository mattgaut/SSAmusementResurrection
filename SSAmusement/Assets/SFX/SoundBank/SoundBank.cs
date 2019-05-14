using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundBank", menuName = "SFX/SoundBank")]
public class SoundBank : ScriptableObject {
    static SoundBank current_soundbank;

    Dictionary<string, SFXClip> sound_effects_dictionary;
    [SerializeField] List<SFXClip> _sound_effect_list;

    public static string GetCodename(string name) {
        name = name.Replace(" ", "_");
        name = name.ToLower();
        name = "sfx_" + name;
        return name;
    }

    public static SFXClip StaticGetSFXClip(string name) {
        return current_soundbank?.GetSFXClip(name);
    }

    public static bool StaticHasSFXClip(string name) {
        return (bool)current_soundbank?.HasSFXClip(name);
    }

    public SFXClip GetSFXClip(string name) {
        if (sound_effects_dictionary.ContainsKey(name)) {
            return sound_effects_dictionary[name];
        }
        return null;
    }

    public bool HasSFXClip(string name) {
        return sound_effects_dictionary.ContainsKey(name);
    }

    public void ReloadDictionary() {
        sound_effects_dictionary = new Dictionary<string, SFXClip>();
        foreach (SFXClip sfx in _sound_effect_list) {
            if (sfx != null)
                sound_effects_dictionary.Add(sfx.codename, sfx);
            else {
                Debug.Log(sfx);
            }
        }
    }

    private void OnEnable() {
        ReloadDictionary();
        if (current_soundbank == null) current_soundbank = this;
    }
}