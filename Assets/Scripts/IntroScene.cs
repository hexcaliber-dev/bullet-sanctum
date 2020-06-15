using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public Graphic textToFade;
    public GameObject splashScreen;
    public bool startDramatically;
    public bool pauseDramatically;
    public float dramaticStartLength;
    public float dramaticPauseLength;
    public float fadeDuration;
    public GameObject nextSlide;
    public bool lastSlide;

    private bool dramaticCalled1; // So the coroutine isn't called more than once
                                    // For th sake of performance
    private bool dramaticCalled2;


    // Start is called before the first frame update
    void Start()
    {
        dramaticCalled1 = true;
        dramaticCalled2 = true;
        textToFade.CrossFadeAlpha(0, 0f, false);
        //if (startDramatically)
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine( endSplash(dramaticPauseLength + dramaticPauseLength + fadeDuration + 1f) );
        if (startDramatically)
        {
            textToFade.CrossFadeAlpha(1, fadeDuration, false);
            if (dramaticCalled1)
                StartCoroutine( dramaticStart(dramaticStartLength) );
        }
        else if (pauseDramatically && dramaticCalled2)
            StartCoroutine(dramaticPause(dramaticPauseLength));
        else if(!pauseDramatically)
        {
            textToFade.CrossFadeAlpha(0, fadeDuration, false);
            fadeDuration -= 0.01f;
        }
    }

    IEnumerator dramaticPause(float pause)
    {
        dramaticCalled2 = false;
        yield return new WaitForSeconds(pause);
        Debug.Log("Paused");
        pauseDramatically = false;
    }

    IEnumerator dramaticStart(float pause)
    {
        dramaticCalled1 = false;
        yield return new WaitForSeconds(pause);
        startDramatically = false;

    }

    IEnumerator endSplash(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (lastSlide)
            SceneManager.LoadScene("0_CaveOfRebirth");
        else
            nextSlide.SetActive(true);
        Destroy(splashScreen);
    }

}
