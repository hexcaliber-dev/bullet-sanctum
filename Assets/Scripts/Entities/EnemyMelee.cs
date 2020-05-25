using UnityEngine;
using System;

public class EnemyMelee : Enemy {

    private Vector3 startPos;
    private Rigidbody2D rb;

    public override void OnSpawn() {
        rb = gameObject.GetComponent<Rigidbody2D>();
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
            var direction = Vector3.zero;
            if (Vector3.Distance(transform.position, playerObj.position) > 0.1) {
                direction = playerObj.position - transform.position;
                rb.AddRelativeForce(direction.normalized * speed, ForceMode2D.Force);
            }
        } else {
            // Nothing lol.
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" && playerFound) {
            rb.AddForce(new Vector2(8, 3), ForceMode2D.Impulse);
        }
    }
}