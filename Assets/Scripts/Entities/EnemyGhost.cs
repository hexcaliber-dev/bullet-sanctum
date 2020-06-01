using System.Collections;
using UnityEngine;

// Basic melee enemy that can go through walls.
public class EnemyGhost : Enemy {

    public float detectRange;
    public float knockback;
    public float STEP_MAX = 5;
    public float decelMultiplier;
    private bool movingRight;
    private float startPos;
    private float endPos;

    public override void OnSpawn () {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        enemyType = EnemyType.Melee;
        movingRight = facingRight;
        startPos = transform.position.x;
        endPos = startPos + STEP_MAX;
    }

    public override void Attack () {
        player.TakeDamage (DMG);
        rb.AddForce (-Vector3.Normalize ((Vector2) (player.transform.position - transform.position)) * knockback);
    }

    // This is called once per update
    public override void PlayerLookout () {
        // print (player.transform.position.x - transform.position.x);
        if (Vector2.Distance (transform.position, player.transform.position) < detectRange) {
            playerFound = true;
        } else {
            playerFound = false;
        }
    }

    // This is called once per update
    public override void MovePattern () {
        // print (playerFound);
        if (playerFound) {
            rb.AddForce (Vector3.Normalize ((Vector2) (player.transform.position - transform.position)) * speed);
        } else {
            rb.velocity *= decelMultiplier;
        }
        // if (Vector2.Distance (player.transform.position, transform.position) < detectRange)
        // print (Vector3.Normalize ((Vector2) (player.transform.position - transform.position)) * speed);
        if (movingRight) {
            if (!facingRight) {
                Flip ();
            }
        }

        if (rb.position.x >= endPos) {
            movingRight = false;
        }

        if (!movingRight) {
            if (facingRight) {
                Flip ();
            }
        }

        if (rb.position.x <= startPos) {
            movingRight = true;
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag == "Player" && playerFound) {
            Attack();
            Vector2 knockback = new Vector2 (8, 3);
            if (facingRight) {
                knockback.x = -knockback.x;
            }
            rb.AddForce (knockback, ForceMode2D.Impulse);
        }
    }
}