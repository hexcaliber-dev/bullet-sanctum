using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player shooting and weapon upgrades.
public class PlayerShoot : MonoBehaviour {
    public Weapon currWeapon;
    public List<Weapon> availableWeapons;
    public List<Sprite> weaponArms, weaponArmsFlipped;
    public List<Vector2> armPos;
    public List<Sprite> muzzleSprites;
    public SpriteRenderer muzzle;
    HUD hud;
    int index;
    Player player;

    void Start () {
        hud = GameObject.FindObjectOfType<HUD> ();
        player = GameObject.FindObjectOfType<Player> ();
        muzzle.enabled = false;
    }

    void Update () {
        if (player.doPlayerUpdates) {
            // Shooting
            if (Input.GetKey (KeyCode.Mouse0)) {
                if (!currWeapon.onCooldown) {
                    hud.UpdateRechargeMeter (currWeapon);
                    currWeapon.UseWeapon ();
                    StartCoroutine(MuzzleFlash());
                }
            }

            // Swap weapons
            if (Input.GetKeyDown (KeyCode.Alpha1)) {
                EquipWeapon (0);
            }
            if (Input.GetKeyDown (KeyCode.Alpha2) && availableWeapons.Count > 1) {
                EquipWeapon (1);
            }
            float scroll = Input.GetAxis ("Mouse ScrollWheel");
            if (scroll != 0f) {
                EquipWeapon ((index + 1) % availableWeapons.Count);
            }
        }
    }

    public void AddWeapon (Weapon weapon) {
        availableWeapons.Add (weapon);
        EquipWeapon (1); // TODO Don't hard code when we add more weapons!!!!!!
    }

    void EquipWeapon (int num) {
        if (availableWeapons.Count > num) {
            currWeapon = availableWeapons[num];
            hud.SwitchWeapon (num);
            index = num;
            player.weaponSprite = weaponArms[num];
            player.weaponSpriteFlipped = weaponArmsFlipped[num];
            Vector2 origPos = player.arm.transform.localPosition;
            player.arm.transform.localPosition = armPos[num];
        }
    }

    IEnumerator MuzzleFlash() {
        muzzle.enabled = true;
        muzzle.sprite = muzzleSprites[0];
        yield return new WaitForSeconds(0.1f);
        muzzle.sprite = muzzleSprites[1];
        yield return new WaitForSeconds(0.1f);
        muzzle.sprite = muzzleSprites[2];
        yield return new WaitForSeconds(0.1f);
        muzzle.enabled = false;
    }
}