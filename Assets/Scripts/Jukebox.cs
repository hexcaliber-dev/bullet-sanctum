using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour {
    public float fadeTime;
    public enum MusicState { Ambient, Intense, Boss }
    public List<AudioClip> ambientClips, intenseClips, bossClips;
    public static MusicState state = MusicState.Ambient;
    AudioSource source;
    int currPlaying = 0;

    // Start is called before the first frame update
    void Start () {
        source = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update () {

    }

    IEnumerator MusicLoop () {
        while (true) {

            int newPlaying = currPlaying;
            while (newPlaying == currPlaying) {
                newPlaying = Random.Range (0, GetClipList ((int) state).Count);
            }
            currPlaying = newPlaying;
            source.clip = GetClipList ((int) state) [newPlaying];
            source.Play ();
            yield return new WaitForSeconds (source.clip.length);
        }
    }

    public void FadeOut () {
        StartCoroutine (cFadeOut ());
    }

    IEnumerator cFadeOut () {
        for (int i = 0; i < 10; i += 1) {
            source.volume -= 0.1f;
            yield return new WaitForSeconds (fadeTime / 10f);
        }
        source.volume = 1f;
    }

    public static void UpdateMusic (MusicState newState) {
        if (newState != state) {
            state = newState;

        }
    }

    List<AudioClip> GetClipList (int num) {
        switch (num) {
            case 0:
                return ambientClips;
            case 1:
                return intenseClips;
            case 2:
                return bossClips;
        }

        Debug.LogWarning ("Invalid music list number: " + num);
        return ambientClips;
    }
}