using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can be shot by player. On destruction, drops specified loot.
public class Chest : LivingEntity {

    public List<Collectible> drops;

    public override void OnSpawn () {
    }

    public override void OnDeath() {
        foreach (Collectible item in drops) {
            Collectible newItem = GameObject.Instantiate(item, transform.position, Quaternion.identity);
            newItem.GetComponent<Rigidbody2D>().velocity = Vector2.up * 4f;
        }
        base.OnDeath();
    }

    // Does nothing
    public override void Attack () {}
}