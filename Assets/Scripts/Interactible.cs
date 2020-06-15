using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// For signs, checkpoint pillars, store.
public abstract class Interactible : MonoBehaviour {
    bool initialized = false;
    bool showingHover = false;
    public TMP_Text hoverText;
    public SpriteRenderer hoverSprite;
    public float hoverTime;
    public float range;
    Player player;

    const int RESOLUTION = 30;

    // Start is called before the first frame update
    protected virtual void Start () {
        hoverText.color = new Color (1f, 1f, 1f, 0f);
        hoverSprite.color = new Color (1f, 1f, 1f, 0f);
        StartCoroutine (FindPlayer ());
    }

    // Update is called once per frame
    virtual protected void Update () {
        if (initialized) {
            if (Vector2.Distance (player.transform.position, transform.position) < range) {
                if (!showingHover) {
                    StartCoroutine (ShowHover ());
                }
                if (Input.GetKeyDown (KeyCode.W)) {
                    Activate ();
                }
            } else {
                if (showingHover) {
                    StartCoroutine (HideHover ());
                }
            }
        }
    }

    abstract protected void Activate ();

    IEnumerator FindPlayer () {
        yield return new WaitForSeconds (0.25f);
        player = GameObject.FindObjectOfType<Player> ();
        initialized = true;
    }

    IEnumerator ShowHover () {
        showingHover = true;
        GetComponent<Animator> ().SetBool ("open", true);
        for (int i = 0; i < RESOLUTION; i += 1) {
            hoverText.color = new Color (1, 1, 1, ((float) i / RESOLUTION));
            yield return new WaitForSeconds (hoverTime / RESOLUTION);
        }
        for (int i = 0; i < RESOLUTION; i += 1) {
            hoverSprite.color = new Color (1, 1, 1, ((float) i / RESOLUTION));
            yield return new WaitForSeconds (hoverTime / RESOLUTION);
        }
    }

    IEnumerator HideHover () {
        showingHover = false;
        GetComponent<Animator> ().SetBool ("open", false);
        for (int i = 0; i < RESOLUTION; i += 1) {
            hoverText.color = new Color (1, 1, 1, 1f - ((float) i / RESOLUTION));
            hoverSprite.color = new Color (1, 1, 1, 1f - ((float) i / RESOLUTION));
            yield return new WaitForSeconds (hoverTime / RESOLUTION);
        }
        hoverText.color = new Color (1f, 1f, 1f, 0f);
        hoverSprite.color = new Color (1f, 1f, 1f, 0f);
    }
}