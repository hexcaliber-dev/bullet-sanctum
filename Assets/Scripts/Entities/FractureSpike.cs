using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can be shot by player. On destruction, drops specified loot.
public class FractureSpike : Chest {

    public int damage; 
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player")) {
            col.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }
}