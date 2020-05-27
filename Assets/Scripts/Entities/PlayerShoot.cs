using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player shooting and weapon upgrades.
public class PlayerShoot : MonoBehaviour {
    public Weapon currWeapon;
    public List<Weapon> availableWeapons;
    HUD hud;
    int index;

    void Start () {
        hud = GameObject.FindObjectOfType<HUD> ();
    }

    void Update () {
        // Shooting
        if (Input.GetKey (KeyCode.Mouse0)) {
            if (!currWeapon.onCooldown) {
                hud.UpdateRechargeMeter (currWeapon);
                currWeapon.UseWeapon ();
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
            EquipWeapon((index + 1) % availableWeapons.Count);
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
        }
    }
}