using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles bounty and bounty multiplier behavior.
public class PlayerBounty : MonoBehaviour {
    public static int bountyMultiplier = 1, savedBounty = 0, unsavedBounty = 0;
    public static int numFragments = 0;
    public List<int> bountyProgressMilestones;
    public static int nextMultiplierProgress = 0;

    HUD hud;

    void Start () {
        hud = GameObject.FindObjectOfType<HUD> ();
        collectBounty (0);
    }

    public void CollectFragment () {
        numFragments += 1;
        hud.fragmentText.text = "x" + numFragments;
    }

    public void collectBounty (int bounty) {
        unsavedBounty += bounty * bountyMultiplier;
        nextMultiplierProgress += bounty * bountyMultiplier;
        if (nextMultiplierProgress >= bountyProgressMilestones[bountyMultiplier]) {
            nextMultiplierProgress = 0;
            bountyMultiplier += 1;
            AudioHelper.PlaySound("multiplier");
        }
        UpdateHudBounty();
    }

    public void BankBounty () {
        savedBounty += unsavedBounty;
        ResetBounty ();
    }
    public void ResetBounty () {
        unsavedBounty = 0;
        bountyMultiplier = 1;
        nextMultiplierProgress = 0;
        UpdateHudBounty();
    }

    public void UpdateHudBounty () {
        hud.UpdateBounty (savedBounty, unsavedBounty, (float) nextMultiplierProgress / bountyProgressMilestones[bountyMultiplier], bountyMultiplier);
    }
}