using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public List<Sprite> strafeSprites, reloadSprites, healthSprites, weaponSprites;
    public Image strafeMeter, reloadMeter, healthMeter, weaponDisplay;

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
}