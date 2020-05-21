using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSwitch : MonoBehaviour
{

    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("RoomSwitch Collision Detected...");
        loadScene();
    }

    void loadScene()
    {
        Debug.Log("Scene Changing to... " + sceneName);
        SceneManager.LoadScene(sceneName);
    }


}
