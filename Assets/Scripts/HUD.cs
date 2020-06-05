using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    public List<Sprite> strafeSprites, reloadSprites, healthSprites, weaponSprites, bountySprites;
    public Sprite overchargeReloadSprite;
    public Image strafeMeter, reloadMeter, secondaryReloadMeter, healthMeter, weaponDisplay, bountyBar, bountyMultBack, fadeImage;
    public CanvasGroup bulletTimeCanvas;
    public TMP_Text bulletTimeText, timer, fragmentText, bountyMultiplierText, bountyText;

    public List<Texture2D> crosshairImages;
    public List<Color> bountyColors;
    int currCrosshair;

    IEnumerator primaryEnumerator, secondaryEnumerator;

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

    IEnumerator ReloadSecondary (Weapon weapon) {
        for (int step = 0; step < reloadSprites.Count; step += 1) {
            secondaryReloadMeter.sprite = reloadSprites[step];
            yield return new WaitForSeconds (weapon.secondaryCooldownTime / reloadSprites.Count);
        }
    }

    public void UpdateRechargeMeter (Weapon weapon) {
        primaryEnumerator = ReloadWeapon(weapon);
        StartCoroutine (primaryEnumerator);
    }
    public void UpdateSecondaryMeter (Weapon weapon) {
        secondaryEnumerator = ReloadSecondary (weapon);
        StartCoroutine (secondaryEnumerator);
    }
    public void ResetPrimaryMeter() {
        if (primaryEnumerator != null)
            StopCoroutine(primaryEnumerator);
        reloadMeter.sprite = reloadSprites[reloadSprites.Count - 1];
    }

    public void OverchargeMeter () {
        print ("OVERCHARGE");
        reloadMeter.sprite = overchargeReloadSprite;
    }

    public void SetSecondary (bool shown) {
        if (secondaryEnumerator != null)
            StopCoroutine (secondaryEnumerator);
        secondaryReloadMeter.enabled = shown;
    }

    public void SetStrafeAmount (int amt) {
        strafeMeter.sprite = strafeSprites[amt];
    }

    public void SetHealthAmount (int health) {
        healthMeter.sprite = healthSprites[health];
    }

    public void SwitchWeapon (int weapon) {
        secondaryReloadMeter.sprite = reloadSprites[reloadSprites.Count - 1];
        weaponDisplay.sprite = weaponSprites[weapon];
        currCrosshair = weapon;
        SetSecondary ((weapon == 0) ? Shop.currPistolUpgrade > 0 : Shop.currShotUpgrade > 0); //TODO fix for more weapons
    }

    public IEnumerator DoBulletTime (float seconds) {
        bulletTimeText.text = "";
        bulletTimeCanvas.alpha = 1f;
        bulletTimeCanvas.GetComponent<Image> ().color = Color.white;
        bulletTimeText.color = Color.white;
        timer.color = Color.white;
        float timeElapsed = 0f;
        bulletTimeText.text = "<_BULLET_TIME";
        while (timeElapsed < seconds) {
            float dec = Mathf.Min (1f, 2f - (timeElapsed / seconds * 2));
            bulletTimeCanvas.GetComponent<Image> ().color = new Color (1, 1, 1, dec / 1.5f);
            bulletTimeText.color = new Color (dec, dec, dec, dec);
            timer.color = new Color (dec, dec, dec, dec);
            timer.text = "00:00:" + (100f - (int) (timeElapsed * 200f) % 100);
            timeElapsed += Time.deltaTime;
            yield return new WaitForSeconds (Time.deltaTime);
        }
        bulletTimeCanvas.alpha = 0f;
        yield return null;
    }

    public void UpdateBounty (int savedBounty, int unsavedBounty, float bountyProgress, int multiplier) {
        bountyMultiplierText.text = "x" + multiplier;
        bountyBar.sprite = bountySprites[(int) (bountyProgress * bountySprites.Count)];
        bountyText.text = savedBounty + "_(+" + unsavedBounty + ")";
        Color bountyColor = bountyColors[multiplier - 1];
        bountyMultBack.color = bountyColor;
        bountyBar.color = bountyColor;
        StartCoroutine (FlashBountyText (bountyColor));
    }

    IEnumerator FlashBountyText (Color col) {
        bountyText.color = col;
        yield return new WaitForSeconds (0.1f);
        bountyText.color = Color.white;
    }
}