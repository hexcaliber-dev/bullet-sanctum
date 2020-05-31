using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles bounty and bounty multiplier behavior.
public class PlayerBounty : MonoBehaviour {
    int bountyMultiplier, savedBounty, unsavedBounty;
    int numFragments;
    public List<int> bountyProgressMilestones;
    int nextMultiplierProgress;

    HUD hud;

    void Start () {
        numFragments = 0;
        savedBounty = 0;
        unsavedBounty = 0;
        bountyMultiplier = 1;
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
        }
        hud.UpdateBounty (savedBounty, unsavedBounty, (float) nextMultiplierProgress / bountyProgressMilestones[bountyMultiplier], bountyMultiplier);
    }

    public void BankBounty () {
        savedBounty += unsavedBounty;
        unsavedBounty = 0;
        bountyMultiplier = 1;
        nextMultiplierProgress = 0;
        hud.UpdateBounty (savedBounty, unsavedBounty, (float) nextMultiplierProgress / bountyProgressMilestones[bountyMultiplier], bountyMultiplier);
    }
}