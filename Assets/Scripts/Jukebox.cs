using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jukebox : MonoBehaviour {
    public float fadeTime;
    // public enum MusicState { Ambient, Intense, Boss }
    // public List<AudioClip> ambientClips, intenseClips, bossClips;
    // public static MusicState state = MusicState.Ambient;
    public static float masterVolume = 1f;
    public static bool needsMusic = true;
    
    AudioSource source;
    int currPlaying = 0;

    // Start is called before the first frame update
    void Start () {
        source = GetComponent<AudioSource> ();
        source.volume = masterVolume;
        SceneManager.sceneLoaded += LoadMusic;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update () {
        source.volume = masterVolume;
    }

    // IEnumerator MusicLoop () {
    //     while (true) {
    //         int newPlaying = currPlaying;
    //         while (newPlaying == currPlaying) {
    //             newPlaying = Random.Range (0, GetClipList ((int) state).Count);
    //         }
    //         currPlaying = newPlaying;
    //         source.clip = GetClipList ((int) state) [newPlaying];
    //         source.Play ();
    //         yield return new WaitForSeconds (source.clip.length);
    //     }
    // }

    public static Jukebox GetJukebox() {
        return GameObject.FindObjectOfType<Jukebox>();
    }

    public void FadeOut () {
        StartCoroutine (cFadeOut ());
    }

    IEnumerator cFadeOut () {
        print("FADE OUT MUSIC");
        for (int i = 0; i < 10; i += 1) {
            source.volume -= 0.1f;
            yield return new WaitForSeconds (fadeTime / 10f);
        }
        source.Stop();
        source.volume = masterVolume;
        needsMusic = true;
    }

    // public static void UpdateMusic (MusicState newState) {
    //     if (newState != state) {
    //         state = newState;

    //     }
    // }

    // List<AudioClip> GetClipList (int num) {
    //     switch (num) {
    //         case 0:
    //             return ambientClips;
    //         case 1:
    //             return intenseClips;
    //         case 2:
    //             return bossClips;
    //     }

    //     Debug.LogWarning ("Invalid music list number: " + num);
    //     return ambientClips;
    // }

    void LoadMusic(Scene scene, LoadSceneMode mode) {
        if (needsMusic) {
            AudioSource musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
            source.clip = musicSource.clip;
            source.Play();
            needsMusic = false;
        }
    }
}