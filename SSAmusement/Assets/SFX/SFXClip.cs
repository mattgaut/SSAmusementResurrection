using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXClip", menuName = "SFX/SFXClip")]
public class SFXClip : ScriptableObject {

    public AudioClip clip { get { return _clip; } }
    public float volume { get { return _volume; } }

    [SerializeField] AudioClip _clip;
    [SerializeField][Range(0, 1)] float _volume;

    public void PlaySound(AudioSource source) {
        source.PlayOneShot(clip, volume);
    }
}
