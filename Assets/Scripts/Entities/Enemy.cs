using UnityEngine;
using System;

public abstract class Enemy : LivingEntity {
    public enum EnemyType { Melee, Ranged, Swarm };
    public EnemyType enemyType;
    public int DMG;
    public Player player;
    public Transform playerObj;
    public Boolean playerFound;
    public Boolean facingRight;

    override protected void Start() {
        facingRight = transform.localScale.x > 0;
        OnSpawn();
    }

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
            if ((playerObj.position.x < transform.position.x && facingRight) ||
                (playerObj.position.x > transform.position.x && !facingRight)) {
                Flip();
            }
        }
    }

    public override void TakeDamage(Bullet b) {
        // Take damage.
    }

    public void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = transform.localScale.x > 0;
    }
}
