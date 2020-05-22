using UnityEngine;
using System;

public abstract class Enemy : LivingEntity {
    public enum EnemyType { Melee, Ranged, Swarm };
    public EnemyType enemyType;
    public int DMG;
    public Player player;
    public Transform playerObj;
    public Boolean playerFound;

    // Runs on Update(). 
    public abstract void MovePattern();

    public void PlayerLookout() {
        if (!playerFound) {
            // Code to see if player is visible by enemy.
            // playerFound = true;
        }
    }

    public void PlayerFound(Boolean state) {
        playerFound = state;
    }

    void Update() {
        // if... AI Stuff & MinMax trees
        MovePattern();
        if (playerFound) {
            if (playerObj.position.x < transform.position.x) {
                transform.rotation = Quaternion.Euler (0, 180, 0);
            } else {
                transform.rotation = Quaternion.identity;
            }
        }
    }

    public override void TakeDamage(Bullet b) {
        // Take damage.
    }
}
