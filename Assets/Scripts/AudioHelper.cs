using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Facilitates all sound effects and music. Only one is allowed per scene.
public class AudioHelper : MonoBehaviour {

    public static Dictionary<string, AudioClip> staticClips;
    public List<AudioClip> audioClips;

    static AudioSource staticWalkingSrc;
    public AudioSource walkingSource;
    public static float masterVolume = 1f;

    private static AudioSource[] audioSources;

    public static void PlaySound (string soundName, bool isLoop, float volume) {
        foreach (AudioSource src in audioSources) {
            if (!src.isPlaying) {
                src.loop = isLoop;
                src.volume = volume * masterVolume;
                src.clip = staticClips[soundName];
                src.Play ();
                return;
            }
        }
    }

    public static void PlaySound (string soundName) {
        PlaySound(soundName, false, 1f);
    }
    public static void PlaySound (string soundName, float volume) {
        PlaySound(soundName, false, volume);
    }

    public static void SetWalking (bool isWalking) {
        if (isWalking) {
           staticWalkingSrc.volume = masterVolume;
        } else if (staticWalkingSrc.volume == masterVolume) {
            staticWalkingSrc.volume = 0f;
            PlaySound("walkend");
        }
    }

    public static void Stop () {
        foreach (AudioSource src in audioSources) {
            src.Stop ();
            src.loop = false;
        }
    }

    // Start is called before the first frame update
    void Start () {
        staticClips = new Dictionary<string, AudioClip> ();
        foreach (AudioClip clip in audioClips) {
            staticClips.Add (clip.name, clip);
        }

        audioSources = GetComponentsInChildren<AudioSource> ();

        if (walkingSource != null)
            staticWalkingSrc = walkingSource;
    }

    // Update is called once per frame
    void Update () {

    }
}