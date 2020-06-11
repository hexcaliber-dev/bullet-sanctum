using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LogoFade : MonoBehaviour
{
    private RawImage logo;
    public GameObject splashScreen;
    public bool startDramatically;
    public bool pauseDramatically;
    public float dramaticStartLength;
    public float dramaticPauseLength;
    public float fadeDuration;

    private bool dramaticCalled1; // So the coroutine isn't called more than once
                                    // For th sake of performance
    private bool dramaticCalled2;


    // Start is called before the first frame update
    void Start()
    {
        logo = GetComponent<RawImage>();
        dramaticCalled1 = true;
        dramaticCalled2 = true;
        logo.CrossFadeAlpha(0, 0f, false);
        //if (startDramatically)
            
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine( endSplash(dramaticPauseLength + dramaticPauseLength + fadeDuration + 1f) );
        if (startDramatically)
        {
            logo.CrossFadeAlpha(1, fadeDuration, false);
            if (dramaticCalled1)
                StartCoroutine( dramaticStart(dramaticStartLength) );
        }
        else if (pauseDramatically && dramaticCalled2)
            StartCoroutine(dramaticPause(dramaticPauseLength));
        else if(!pauseDramatically)
        {
            logo.CrossFadeAlpha(0, fadeDuration, false);
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
        Destroy(splashScreen);
    }

}
