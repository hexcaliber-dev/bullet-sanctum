using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player shooting and weapon upgrades.
public class PlayerShoot : MonoBehaviour {
    public static int currWeapon = 0;
    public List<Weapon> allWeapons;
    public static List<Weapon> availableWeapons;
    public static bool unitializedAvailableWeapons = true;
    public Weapon defaultWeapon;
    
    public List<Sprite> pistolArms, shotArms, pistolArmsFlipped, shotArmsFlipped;
    public List<Vector2> armPos;
    HUD hud;
    Player player;

    void Start () {
        if (unitializedAvailableWeapons)
        {
            Debug.Log("Initialize Avaliable Weapons");
            availableWeapons = new List<Weapon>() {defaultWeapon};
            unitializedAvailableWeapons = false;
        }
        hud = GameObject.FindObjectOfType<HUD> ();
        player = GameObject.FindObjectOfType<Player> ();
        EquipWeapon (currWeapon);
    }

    void Update () {
        if (player.doPlayerUpdates) {
            // Shooting
            if (Input.GetKey (KeyCode.Mouse0)) {
                if (!availableWeapons[currWeapon].onCooldown) {
                    Debug.Log(availableWeapons[currWeapon]);
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

    // Pick up a weapon from the ground.
    public void GetNewWeapon (int weapon) {
        if (weapon == availableWeapons.Count) {
            availableWeapons.Add (allWeapons[weapon]);
            EquipWeapon(weapon);
        } else {
            Debug.LogWarning("Picked up invalid weapon: " + weapon);
        }
    }

    void EquipWeapon (int num) {
        if (availableWeapons.Count > num) {
            currWeapon = num;
            hud.SwitchWeapon (num);
            player.weaponSprite = (num == 0) ? pistolArms[0] : shotArms[0];
            player.weaponSpriteFlipped = (num == 0) ? pistolArmsFlipped[0] : shotArmsFlipped[0];
            Vector2 origPos = player.arm.transform.localPosition;
            player.arm.transform.localPosition = armPos[num];
        }
    }

    IEnumerator MuzzleFlash () {
        List<Sprite> arms = (currWeapon == 0) ? pistolArms : shotArms;
        List<Sprite> armsFlipped = (currWeapon == 0) ? pistolArmsFlipped : shotArmsFlipped;
        player.weaponSprite = arms[1];
        player.weaponSpriteFlipped = armsFlipped[1];
        yield return new WaitForSeconds (0.1f);
        player.weaponSprite = arms[2];
        player.weaponSpriteFlipped = armsFlipped[2];
        yield return new WaitForSeconds (0.1f);
        player.weaponSprite = arms[3];
        player.weaponSpriteFlipped = armsFlipped[3];
        yield return new WaitForSeconds (0.1f);
        player.weaponSprite = arms[0];
        player.weaponSpriteFlipped = armsFlipped[0];
    }
}