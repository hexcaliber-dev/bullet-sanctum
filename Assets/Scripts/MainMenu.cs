using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public string levelName;
    // Start is called before the first frame update
    void Start () {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update () {

    }

    public void LoadArena() {
        SceneManager.LoadScene(levelName);
    }
}