using UnityEngine;
using System;

public abstract class Enemy : LivingEntity {
    public enum EnemyType { Melee, Ranged, Swarm };
    public EnemyType enemyType;
    public int DMG;
    public Player player;
    public Boolean playerFound;

    // Runs on Update(). 
    public abstract void MovePattern();

    public void PlayerLookout() {
        if (!playerFound) {
            // Code to see if player is visible by enemy.
            // playerFound = true;
        }
    }

    void Update() {
        // if... AI Stuff & MinMax trees
        PlayerLookout();
        MovePattern();
    }

    public override void TakeDamage(Bullet b) {
        // Take damage.
    }
}
