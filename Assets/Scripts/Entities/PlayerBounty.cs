using UnityEngine;

// Handles bounty and bounty multiplier behavior.
public class PlayerBounty : MonoBehaviour {
    int bountyMultiplier, savedBounty, unsavedBounty;
    int numFragments;

    HUD hud;

    void Start() {
        numFragments = 0;
        savedBounty = 0;
        unsavedBounty = 0;
        bountyMultiplier = 1;
        hud = GameObject.FindObjectOfType<HUD>();
    }

    public void CollectFragment() {
        numFragments += 1;
        hud.fragmentText.text = "x" + numFragments;
    }
}