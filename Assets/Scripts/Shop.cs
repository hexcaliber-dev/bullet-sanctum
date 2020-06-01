﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Interactible {
    public CanvasGroup shopPanel, upgradeInfoPanel;
    public TMP_Text upgradeInfoText, upgradeCostText, upgradeDescText;
    public List<string> pistolNames, shotNames;
    public List<TextAsset> pistolDescs, shotDescs;
    public List<int> pistolCosts, shotCosts;
    public List<Image> pistolBars, shotBars;

    const int NUM_UPGRADES = 4; // counts the default loadout
    HUD hud;
    Player player;

    public static int currPistolUpgrade = 0, currShotUpgrade = 0;

    protected override void Start () {
        base.Start ();
        hud = GameObject.FindObjectOfType<HUD> ();
        player = GameObject.FindObjectOfType<Player> ();
        UpdateWeaponBars ();
        Close ();
    }

    protected override void Activate () {
        player = GameObject.FindObjectOfType<Player> ();
        shopPanel.alpha = 1f;
        hud.doCursorDraw = false;
        shopPanel.blocksRaycasts = true;
        player.doPlayerUpdates = false;
    }

    // 0 is pistol, 1 is shotgun. Click behavior
    public void Upgrade (int weapon) {
        if ((weapon == 0) ? currPistolUpgrade <= NUM_UPGRADES : currShotUpgrade < NUM_UPGRADES) {
            int cost = (weapon == 0) ? pistolCosts[currPistolUpgrade] : shotCosts[currShotUpgrade];
            if (PlayerBounty.savedBounty + PlayerBounty.unsavedBounty >= cost) {
                int diff = Mathf.Max (PlayerBounty.unsavedBounty - cost, -1);
                if (diff >= 0) {
                    PlayerBounty.unsavedBounty -= cost;
                } else {
                    PlayerBounty.savedBounty -= (cost - PlayerBounty.unsavedBounty);
                    PlayerBounty.unsavedBounty = 0;
                }
                GameObject.FindObjectOfType<PlayerBounty> ().UpdateHudBounty ();
                if (weapon == 0) {
                    currPistolUpgrade += 1;
                    GameObject.FindObjectOfType<Pistol> ().onSecondaryCooldown = false;
                    print ("Upgraded pistol to " + currPistolUpgrade);
                } else {
                    currShotUpgrade += 1;
                    GameObject.FindObjectOfType<Shotgun> ().onSecondaryCooldown = false;
                    print ("Upgraded shotgun to " + currShotUpgrade);
                }
                hud.SwitchWeapon (PlayerShoot.currWeapon);
            }
            UpdateWeaponBars ();
        }
    }

    public void ShowDetails (int weapon) {
        upgradeInfoPanel.alpha = 1f;
        if ((weapon == 0) ? currPistolUpgrade == NUM_UPGRADES - 1 : currShotUpgrade == NUM_UPGRADES - 1) {
            upgradeInfoText.text = "MAX_UPGRADE";
            upgradeCostText.text = "";
            upgradeDescText.text = "";
        } else {
            if (weapon == 0) {
                upgradeInfoText.text = pistolNames[currPistolUpgrade];
                upgradeCostText.text = pistolCosts[currPistolUpgrade] + "_BOUNTY";
                upgradeDescText.text = pistolDescs[currPistolUpgrade].text.Replace (' ', '_');
            } else {
                upgradeInfoText.text = shotNames[currShotUpgrade];
                upgradeCostText.text = shotCosts[currShotUpgrade] + "_BOUNTY";
                upgradeDescText.text = shotDescs[currShotUpgrade].text.Replace (' ', '_');
            }
        }
    }

    public void HideDetails () {
        upgradeInfoPanel.alpha = 0f;
    }

    public void Close () {
        shopPanel.alpha = 0f;
        hud.doCursorDraw = true;
        shopPanel.blocksRaycasts = false;
        if (player != null)
            player.doPlayerUpdates = true;
    }

    public void UpdateWeaponBars () {
        for (int i = 0; i < NUM_UPGRADES; i += 1) {
            if (i <= currPistolUpgrade) {
                pistolBars[i].color = Color.white;
            } else {
                pistolBars[i].color = new Color (0.3f, 0.3f, 0.3f, 1);
            }
            if (i <= currShotUpgrade) {
                shotBars[i].color = Color.white;
            } else {
                shotBars[i].color = new Color (0.3f, 0.3f, 0.3f, 1);
            }
        }
    }
}