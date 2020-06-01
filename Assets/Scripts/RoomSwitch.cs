using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomSwitch : MonoBehaviour
{
    private static string currentId = "";
    public float transitionSpeed;
    public Image fadeImage;
    public GameObject player; 

    public GameObject playerSpawnPoint;
    public GameObject alternateSpawnPoint;
    // public GameObject cameraFollower;
    public string switcherID;
    public string nextSwitcherID;
    bool switchNow;
    public bool alternate;
    public string sceneName;
    private bool activeSwitcher;
    public bool defaultSpawn;


// To setup spawn locations for each room, you must do the following:
// Make an empty object
// Under that have another object with this script
// To setup a regular spawn point, make an empty object under the object
// with the script, then attatch it to playerSpawnPoint variable in the script
// Same for alternate except it doesn't need to be a child

// NOTE: Alternate has precedence, but once it's use, the alternate bool is set to
// false
    

    // Start is called before the first frame update
    void Awake()
    {
        switchNow = false;
        activeSwitcher = false;

        if (currentId == "")
        {
            if (defaultSpawn == true)
            {
                currentId = switcherID;
            }
        }
        if (currentId == switcherID)
        {
            activeSwitcher = true;
        }

        if (activeSwitcher)
        {
            fadeImage.color = new Color32(0, 0, 0, 255);
            if (alternate)
            {
                if (alternateSpawnPoint != null)
                    spawnPlayer(alternateSpawnPoint);
                else
                    Debug.Log("Set Alternate SpawnPoint!");
                alternate = false;
            }
            else
            {
                if (playerSpawnPoint != null)
                    spawnPlayer(playerSpawnPoint);
                else
                    Debug.Log("Set Regular SpawnPoint!");
            }
            StartCoroutine(fadeIn());
        }

        activeSwitcher = false;
        

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
        // Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("RoomSwitch Collision Detected...");
            StartCoroutine(fadeOut());
        }
    }

    void loadScene()
    {
        currentId = nextSwitcherID;
        Debug.Log("Scene Changing to... " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator fadeOut()
    {
        Debug.Log("Fading Out... Start");

        player.GetComponent<Player>().enabled = false;
        for (float i = 0; i < 255; i += transitionSpeed)
        {
            fadeImage.color = new Color32(0, 0, 0, (byte)i);
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
        for (float i = 255; i > 0; i -= transitionSpeed)
        {
            
            fadeImage.color = new Color32(0, 0, 0, (byte)i);
            yield return null;
        }
        player.GetComponent<Player>().enabled = true;
        
        Debug.Log("Fading In... End");
    }

    void spawnPlayer(GameObject t)
    {
        Debug.Log("Player Spawned");
        GameObject sp = Instantiate(player, t.transform);
        sp.GetComponent<Player>().enabled = true;
        //setCameraTarget(sp);

    }

    // void setCameraTarget(GameObject p)
    // {
    //     cameraFollower.GetComponent<CameraUtils>().SetPlayer(p);
    // }


}
