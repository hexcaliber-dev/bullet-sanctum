using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public List<Sprite> strafeSprites, reloadSprites, healthSprites, weaponSprites;
    public Image strafeMeter, reloadMeter, healthMeter, weaponDisplay;
    public CanvasGroup bulletTimeCanvas;
    public TMP_Text bulletTimeText, timer, fragmentText;

    public List<Texture2D> crosshairImages;
    int currCrosshair;

    public bool doCursorDraw;

    // Start is called before the first frame update
    void Start () {
        Cursor.visible = false;
        doCursorDraw = true;
    }

    // Update is called once per frame
    void Update () { }

    // Make custom cursor
    void OnGUI () {
        if (doCursorDraw) {
            float xMin = (Input.mousePosition.x) - (crosshairImages[currCrosshair].width / 2);
            float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImages[currCrosshair].height / 2);
            // print (new Vector2 (xMin, yMin));
            GUI.DrawTexture (new Rect (xMin, yMin, crosshairImages[currCrosshair].width, crosshairImages[currCrosshair].height), crosshairImages[currCrosshair]);
        }
    }

    IEnumerator ReloadWeapon (Weapon weapon) {
        for (int step = 0; step < reloadSprites.Count; step += 1) {
            reloadMeter.sprite = reloadSprites[step];
            yield return new WaitForSeconds (weapon.cooldownTime / reloadSprites.Count);
        }
    }

    public void UpdateRechargeMeter (Weapon weapon) {
        StartCoroutine (ReloadWeapon (weapon));
    }

    public void SetStrafeAmount (int amt) {
        strafeMeter.sprite = strafeSprites[amt];
    }

    public void SetHealthAmount (int health) {
        healthMeter.sprite = healthSprites[health];
    }

    public void SwitchWeapon (int weapon) {
        weaponDisplay.sprite = weaponSprites[weapon];
        currCrosshair = weapon;
    }

    public IEnumerator DoBulletTime (float seconds) {
        bulletTimeText.text = "";
        bulletTimeCanvas.alpha = 1f;
        bulletTimeCanvas.GetComponent<Image> ().color = Color.white;
        bulletTimeText.color = Color.white;
        timer.color = Color.white;
        for (int i = 0; i < 100; i += 1) {
            float dec = Mathf.Min(1f, 2f - (i / 50f));
            bulletTimeCanvas.GetComponent<Image> ().color = new Color (1, 1, 1, dec / 1.5f);
            bulletTimeText.color = new Color (dec, dec, dec, dec);
            timer.color = new Color (dec, dec, dec, dec);
            timer.text = "00:0" + (int)(seconds * i / 3f) + ":" + (((int)(seconds * i * 30f) % 90f + 10));
            if (i == 10)
                bulletTimeText.text = "BULLET_";
            if (i == 30)
                bulletTimeText.text += "TIME_";
            if (i == 50)
                bulletTimeText.text += "<_";
            yield return new WaitForSeconds (seconds / 100);
        }
        bulletTimeCanvas.alpha = 0f;
    }
}