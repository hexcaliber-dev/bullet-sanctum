using UnityEngine;
using System;

public class EnemyMelee : Enemy {

    private Vector3 startPos;

    public override void OnSpawn() {
        enemyType = EnemyType.Melee;
        PlayerLookout();
    }

    public override void Attack() {
        player.health -= DMG;
    }
    public override void OnDeath() {
        // Fucking die.
    }
    public override void MovePattern() {
        if (playerFound) {
            transform.position = Vector3.MoveTowards(transform.position, playerObj.position, speed*Time.deltaTime);
        } else {
            // Nothing lol.
        }
    }
}