using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    // Start is called before the first frame update
    public List<CanvasGroup> panels;
    public CanvasGroup menuPanel;
    public TMP_Text header, musicNum, effectsNum;

    float prevTime;

    void Start () { }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            ToggleMenu ();
        }
    }

    public void ShowPanel (int panel) {
        HideAllPanels ();
        panels[panel].alpha = 1f;
        panels[panel].blocksRaycasts = true;
        panels[panel].interactable = true;
    }

    public void HideAllPanels () {
        for (int i = 0; i < panels.Count; i += 1) {
            panels[i].alpha = 0f;
            panels[i].blocksRaycasts = false;
            panels[i].interactable = false;
        }
    }

    public void SetHeaderText (string text) {
        header.text = text;
    }

    public void HoverOn (TMP_Text textAsset) {
        textAsset.color = Color.black;
    }

    public void HoverOff (TMP_Text textAsset) {
        textAsset.color = Color.white;
    }

    public void QuitToMenu () {
        SceneManager.LoadScene ("MainMenu");
    }
    public void QuitToDesktop () {
        Application.Quit ();
    }

    public void ToggleMenu () {
        if (menuPanel.alpha == 1f) {
            HideMenu ();
        } else {
            HideAllPanels ();
            menuPanel.alpha = 1f;
            menuPanel.blocksRaycasts = true;
            menuPanel.interactable = true;
            prevTime = Time.timeScale;
            Time.timeScale = 0f;
            GetPlayer ().doPlayerUpdates = false;
            GetHUD ().doCursorDraw = false;
            Cursor.visible = true;
            SetHeaderText("");
        }
    }

    public void HideMenu () {
        menuPanel.alpha = 0f;
        menuPanel.blocksRaycasts = false;
        menuPanel.interactable = false;
        Time.timeScale = prevTime;
        GetPlayer ().doPlayerUpdates = true;
        GetHUD ().doCursorDraw = true;
        Cursor.visible = false;
    }

    public void AddVolume (float amount) {
        AudioHelper.masterVolume = Mathf.Clamp (AudioHelper.masterVolume + amount, 0f, 1f);
        effectsNum.text = (int) (AudioHelper.masterVolume * 100) + "%";

    }

    public void AddMusicVolume (float amount) {
        Jukebox.masterVolume = Mathf.Clamp (Jukebox.masterVolume + amount, 0f, 1f);
        musicNum.text = (int) (Jukebox.masterVolume * 100) + "%";
    }

    public void Suicide () {
        HideMenu();
        GetPlayer ().OnDeath ();
    }

    Player GetPlayer () {
        return GameObject.FindObjectOfType<Player> ();
    }

    HUD GetHUD () {
        return GameObject.FindObjectOfType<HUD> ();
    }

    public void StartGame() {
        SceneManager.LoadScene("IntroScene");
    }
}