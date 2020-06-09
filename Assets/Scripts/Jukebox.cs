using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Jukebox : MonoBehaviour {
    public float fadeTime;
    public enum MusicState {Ambient, Intense, Boss};
    public List<AudioClip> ambientClips, intenseClips, bossClips;
    public static MusicState state = MusicState.Ambient;
    AudioSource source;

    // Start is called before the first frame update
    void Start () {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update () {

    }

    IEnumerator MusicLoop() {
        while(true) {
            yield return new WaitForSeconds(1); //TODO
        }
    }

    public void FadeOut() {
        StartCoroutine(cFadeOut());
    }

    IEnumerator cFadeOut() {
        for(int i = 0; i < 10; i += 1) {
            source.volume -= 0.1f;
            yield return new WaitForSeconds(fadeTime / 10f);
        }
        source.volume = 1f;
    }

    public static void UpdateMusic(MusicState newState) {
        if (newState != state) {
            state = newState;

        }
    }
}