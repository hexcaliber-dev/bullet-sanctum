using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player shooting and weapon upgrades.
public class PlayerShoot : MonoBehaviour {
    public static int currWeapon = 0;
    public List<Weapon> availableWeapons;
    public List<Sprite> weaponArms, weaponArmsFlipped;
    public List<Vector2> armPos;
    public List<Sprite> muzzleSprites;
    public SpriteRenderer muzzle;
    HUD hud;
    Player player;

    void Start () {
        hud = GameObject.FindObjectOfType<HUD> ();
        player = GameObject.FindObjectOfType<Player> ();
        muzzle.enabled = false;
        EquipWeapon (currWeapon);
    }

    void Update () {
        if (player.doPlayerUpdates) {
            // Shooting
            if (Input.GetKey (KeyCode.Mouse0)) {
                if (!availableWeapons[currWeapon].onCooldown) {
                    hud.UpdateRechargeMeter (availableWeapons[currWeapon]);
                    availableWeapons[currWeapon].UseWeapon ();
                    StartCoroutine (MuzzleFlash ());
                }
            }

            // Secondary fire
            if (Input.GetKey (KeyCode.Mouse1)) {
                print (availableWeapons[currWeapon].onSecondaryCooldown);
                if (!availableWeapons[currWeapon].onSecondaryCooldown) {
                    availableWeapons[currWeapon].UseSecondary ();
                    hud.UpdateSecondaryMeter (availableWeapons[currWeapon]);
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
                EquipWeapon ((currWeapon + 1) % availableWeapons.Count);
            }
        }
    }

    public void AddWeapon (Weapon weapon) {
        availableWeapons.Add (weapon);
        EquipWeapon (1); // TODO Don't hard code when we add more weapons!!!!!!
    }

    void EquipWeapon (int num) {
        if (availableWeapons.Count > num) {
            currWeapon = num;
            hud.SwitchWeapon (num);
            hud.UpdateRechargeMeter (availableWeapons[num]);
            hud.UpdateSecondaryMeter (availableWeapons[num]);
            player.weaponSprite = weaponArms[num];
            player.weaponSpriteFlipped = weaponArmsFlipped[num];
            Vector2 origPos = player.arm.transform.localPosition;
            player.arm.transform.localPosition = armPos[num];
        }
    }

    IEnumerator MuzzleFlash () {
        muzzle.enabled = true;
        muzzle.sprite = muzzleSprites[0];
        yield return new WaitForSeconds (0.1f);
        muzzle.sprite = muzzleSprites[1];
        yield return new WaitForSeconds (0.1f);
        muzzle.sprite = muzzleSprites[2];
        yield return new WaitForSeconds (0.1f);
        muzzle.enabled = false;
    }
}