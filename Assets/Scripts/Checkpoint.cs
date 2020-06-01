using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactible {
    public static int lastCheckpoint = 5; // TODO set to initial spawnpoint
    public int checkpointID; // Set to be the same as the room ID
    protected override void Activate () {
        GameObject.FindObjectOfType<PlayerBounty> ().BankBounty ();
        lastCheckpoint = checkpointID;
    }
}