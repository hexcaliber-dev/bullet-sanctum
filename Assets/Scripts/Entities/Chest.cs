using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can be shot by player. On destruction, drops specified loot.
public class Chest : LivingEntity {

    public List<Collectible> drops;
    public int id;

    public static List<int> lootedChests = new List<int>();

    protected override void Start() {
        if (lootedChests.Contains(id)) {
            Destroy(gameObject);
        }
    }

    public override void OnSpawn() {}

    public override void OnDeath() {
        foreach (Collectible item in drops) {
            Collectible newItem = GameObject.Instantiate(item, transform.position, Quaternion.identity);
            newItem.GetComponent<Rigidbody2D>().velocity = Vector2.up * 4f;
        }
        lootedChests.Add(id);
        base.OnDeath();
    }

    // Does nothing
    public override void Attack () {}
}