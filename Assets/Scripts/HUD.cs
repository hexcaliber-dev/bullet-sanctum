using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public List<Sprite> strafeSprites, reloadSprites, healthSprites;
    public Image strafeMeter, reloadMeter, healthMeter;

    public Texture2D crosshairImage;

    // Start is called before the first frame update
    void Start () {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update () { }

    // Make custom cursor
    void OnGUI () {
        float xMin = (Input.mousePosition.x) - (crosshairImage.width / 2);
        float yMin = (Screen.height - Input.mousePosition.y) - (crosshairImage.height / 2);
        print (new Vector2 (xMin, yMin));
        GUI.DrawTexture (new Rect (xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
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
}