using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomSwitch : MonoBehaviour {
    public static string currentId = "";
    private static Vector3 spawnPosition = new Vector3 (0.0f, 0.0f, 0f);
    private static string checkpointScene;
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
    public bool fadeMusic;
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
    void Awake () {
        switchNow = false;
        activeSwitcher = false;

        // if(currentId == "death")
        // {
        //     currentId = "justSpawned";
        //     print("PLAYER SPAWN BEGIN");
        //     GameObject sp = GameObject.Instantiate(new GameObject("PlayerHolder"), Checkpoint.getSpawnPosition(), Quaternion.identity);
        //     // sp.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        //     spawnPlayer(sp);
        //     StartCoroutine(fadeIn());
        // }

        if (currentId == "") {
            if (defaultSpawn == true) {
                currentId = switcherID;
            }
        }
        if (currentId == switcherID) {
            activeSwitcher = true;
        }

        if (activeSwitcher) {
            fadeImage.color = new Color32 (0, 0, 0, 255);
            if (alternate) {
                if (alternateSpawnPoint != null)
                    spawnPlayer (alternateSpawnPoint);
                else
                    Debug.Log ("Set Alternate SpawnPoint!");
                alternate = false;
            } else {
                if (playerSpawnPoint != null)
                    spawnPlayer (playerSpawnPoint);
                else
                    Debug.Log ("Set Regular SpawnPoint!");
            }
            StartCoroutine (fadeIn ());
        }

        activeSwitcher = false;

    }

    // Update is called once per frame
    void Update () {
        if (currentId == "death") {
            switchNow = true;
        }
        if (switchNow) {
            loadScene ();
        }
    }

    void OnTriggerEnter2D (Collider2D col) {
        // Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag == "Player") {
            Debug.Log ("RoomSwitch Collision Detected...");
            activateSwitch ();
        }
    }

    void activateSwitch () {
        StartCoroutine (fadeOut ());
    }

    public static void OnPlayerDeath () {
        currentId = "death";

    }

    void loadScene () {
        if (currentId == "death") {
            Debug.Log ("Scene Changing to... " + Checkpoint.getCurrentCheckpoint ());
            SceneManager.LoadScene (Checkpoint.getCurrentCheckpoint ());
            return;
        }
        currentId = nextSwitcherID;
        Debug.Log ("Scene Changing to... " + sceneName);
        SceneManager.LoadScene (sceneName);
    }

    IEnumerator fadeOut () {
        Debug.Log ("Fading Out... Start");
        if (fadeMusic) {
            Jukebox.GetJukebox().FadeOut();
        }

        GameObject.FindObjectOfType<Player>().doPlayerUpdates = false;
        for (float i = 0; i < 255; i += transitionSpeed) {
            fadeImage.color = new Color32 (0, 0, 0, (byte) i);
            yield return null;
        }
        GameObject.FindObjectOfType<Player>().doPlayerUpdates = false;

        switchNow = true;
        Debug.Log ("Fading Out... End");
    }

    IEnumerator fadeIn () {
        Debug.Log ("Fading In... Start");

        player.GetComponent<Player> ().enabled = false;
        for (float i = 255; i > 0; i -= transitionSpeed) {

            fadeImage.color = new Color32 (0, 0, 0, (byte) i);
            yield return null;
        }
        player.GetComponent<Player> ().enabled = true;

        Debug.Log ("Fading In... End");
    }

    void spawnPlayer (GameObject t) {
        Debug.Log ("Player Spawned");
        GameObject sp = Instantiate (player, t.transform);
        sp.GetComponent<Player> ().enabled = true;
        //setCameraTarget(sp);

    }

    // void setCameraTarget(GameObject p)
    // {
    //     cameraFollower.GetComponent<CameraUtils>().SetPlayer(p);
    // }

}