using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Used for tutorial popups, text boxes, and other trigger events. Attach to a Collider2D object.
public class EventTrigger : MonoBehaviour {
    public bool reusable;
    public CanvasGroup popup;
    public TMP_Text pressEnterText;

    const float FADE_TIME = 1f,
        ENTER_DELAY = 2f;

    // number of opacity increments during fade. Very high numbers may break the animation!
    const int FADE_RESOLUTION = 30;

    private bool activated;

    void Start () {
        pressEnterText.color = new Color (1, 1, 1, 0);
    }

    void Update () {
        if (activated && Input.GetKeyDown (KeyCode.Return)) {
            StartCoroutine (FadeOut ());
        }
    }

    void OnTriggerEnter2D (Collider2D col) {
        if (col.gameObject.layer == LayerMask.NameToLayer ("Player")) {
            print (col.gameObject.layer);
            StartCoroutine (FadeIn ());
        }
    }

    private IEnumerator FadeIn () {
        GameObject.FindObjectOfType<Player> ().doPlayerUpdates = false;
        GameObject.FindObjectOfType<HUD> ().doCursorDraw = false;
        for (int i = 0; i < FADE_RESOLUTION; i += 1) {
            popup.alpha += 1.0f / FADE_RESOLUTION;
            yield return new WaitForSeconds (FADE_TIME / FADE_RESOLUTION);
        }
        yield return new WaitForSeconds (ENTER_DELAY);
        for (int i = 0; i < FADE_RESOLUTION; i += 1) {
            pressEnterText.color = new Color (1, 1, 1, (float) (i) / FADE_RESOLUTION);
            yield return new WaitForSeconds (FADE_TIME / FADE_RESOLUTION);
        }

        activated = true;
    }

    private IEnumerator FadeOut () {
        for (int i = 0; i < FADE_RESOLUTION; i += 1) {
            popup.alpha -= 1.0f / FADE_RESOLUTION;
            yield return new WaitForSeconds (FADE_TIME / FADE_RESOLUTION);
        }
        activated = false;
        GameObject.FindObjectOfType<Player> ().doPlayerUpdates = true;
        GameObject.FindObjectOfType<HUD> ().doCursorDraw = true;
        if (!reusable) {
            Destroy (gameObject);
        }
    }
}