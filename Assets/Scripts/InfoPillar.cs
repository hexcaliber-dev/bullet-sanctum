using System.Collections;
using TMPro;
using UnityEngine;

public class InfoPillar : Interactible {
    public TextAsset text;
    public float fadeTime, enterDelay;
    public CanvasGroup popup;
    public TMP_Text pressEnterText;
    public TMP_Text content;
    // number of opacity increments during fade. Very high numbers may break the animation!
    const int FADE_RESOLUTION = 30;
    private bool activated;

    protected override void Activate () {
        if (!activated)
            StartCoroutine (FadeIn ());
    }

    protected override void Start () {
        pressEnterText.color = new Color (1, 1, 1, 0);
        content.text = text.text.ToUpper ();
        base.Start ();
    }

    protected override void Update () {
        base.Update ();
        if (activated && Input.GetKeyDown (KeyCode.W)) {
            StartCoroutine (FadeOut ());
        }
    }

    private IEnumerator FadeIn () {
        GameObject.FindObjectOfType<Player> ().doPlayerUpdates = false;
        GameObject.FindObjectOfType<HUD> ().doCursorDraw = false;
        for (int i = 0; i < FADE_RESOLUTION; i += 1) {
            popup.alpha += 1.0f / FADE_RESOLUTION;
            yield return new WaitForSeconds (fadeTime / FADE_RESOLUTION);
        }
        yield return new WaitForSeconds (enterDelay);
        for (int i = 0; i < FADE_RESOLUTION; i += 1) {
            pressEnterText.color = new Color (1, 1, 1, (float) (i) / FADE_RESOLUTION);
            yield return new WaitForSeconds (fadeTime / FADE_RESOLUTION);
        }

        activated = true;
    }

    private IEnumerator FadeOut () {
        for (int i = 0; i < FADE_RESOLUTION; i += 1) {
            popup.alpha -= 1.0f / FADE_RESOLUTION;
            yield return new WaitForSeconds (fadeTime / FADE_RESOLUTION);
        }
        activated = false;
        GameObject.FindObjectOfType<Player> ().doPlayerUpdates = true;
        GameObject.FindObjectOfType<HUD> ().doCursorDraw = true;
        pressEnterText.color = new Color (1, 1, 1, 0);
    }
}