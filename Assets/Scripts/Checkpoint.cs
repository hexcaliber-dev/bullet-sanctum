using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactible {

    protected override void Activate () {
        GameObject.FindObjectOfType<PlayerBounty>().BankBounty();
    }
}