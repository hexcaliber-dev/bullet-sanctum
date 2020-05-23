using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomSwitch : MonoBehaviour
{

    public byte transitionSpeed;
    public Image fadeImage;
    public GameObject player; 

    bool switchNow;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        switchNow = false;
        fadeImage.color = new Color32(0, 0, 0, 255);
        StartCoroutine(fadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if (switchNow)
        {
            loadScene();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("RoomSwitch Collision Detected...");
            StartCoroutine(fadeOut());
        }
    }

    void loadScene()
    {
        Debug.Log("Scene Changing to... " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator fadeOut()
    {
        Debug.Log("Fading Out... Start");

        player.GetComponent<Player>().enabled = false;
        for (byte i = 0; i < 255; i += transitionSpeed)
        {
            fadeImage.color = new Color32(0, 0, 0, i);
            yield return null;
        }
        player.GetComponent<Player>().enabled = true;
        
        switchNow = true;
        Debug.Log("Fading Out... End");
    }

    IEnumerator fadeIn()
    {
        Debug.Log("Fading In... Start");

        player.GetComponent<Player>().enabled = false;
        for (byte i = 255; i > 0; i -= transitionSpeed)
        {
            fadeImage.color = new Color32(0, 0, 0, i);
            yield return null;
        }
        player.GetComponent<Player>().enabled = true;
        
        Debug.Log("Fading In... End");
    }


}
