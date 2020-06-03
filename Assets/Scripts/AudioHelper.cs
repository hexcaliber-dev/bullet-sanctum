using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Facilitates all sound effects and music. Only one is allowed per scene.
public class AudioHelper : MonoBehaviour {

    public static Dictionary<string, AudioClip> staticClips;
    public List<AudioClip> audioClips;

    private static AudioSource[] audioSources;

    public static void PlaySound (string soundName, bool isLoop) {
        foreach (AudioSource src in audioSources) {
            if (!src.isPlaying) {
                src.loop = isLoop;
                src.clip = staticClips[soundName];
                src.Play ();
                return;
            }
        }
    }

    public static void PlaySound (string soundName) {
        PlaySound(soundName, false);
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
    }

    // Update is called once per frame
    void Update () {

    }
}