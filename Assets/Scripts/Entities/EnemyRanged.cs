using UnityEngine;
using System;

// Basic ranged enemy that requires players to be on the same level to attack.
// Cannot go through walls.
public class EnemyRanged : Enemy {

    public float xRange, yRange;
    
    private Rigidbody2D rb;
    public float STEP_MAX = 5;
    private bool movingRight;
    private float startPos;
    private float endPos;

    public override void OnSpawn() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        enemyType = EnemyType.Melee;
        PlayerLookout();
        movingRight = facingRight;
        startPos = transform.position.x;
        endPos = startPos + STEP_MAX;
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
            if (Vector3.Distance(transform.position, player.transform.position) > 0.1) {
                direction = player.transform.position - transform.position;
                rb.AddRelativeForce(direction.normalized * speed, ForceMode2D.Force);
            }
        } else {
            if (movingRight) {
                rb.AddForce(Vector2.right);
                if (!facingRight) {
                    Flip();
                }
            }

            if (rb.position.x >= endPos) {
                movingRight = false;
            }

            if (!movingRight) {
                rb.AddForce(-Vector2.right);
                if (facingRight) {
                    Flip();
                }
            }

            if (rb.position.x <= startPos) {
                movingRight = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" && playerFound) {
            Vector2 knockback = new Vector2(8, 3);
            if (facingRight) {
                knockback.x = -knockback.x;
            }
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }
}
