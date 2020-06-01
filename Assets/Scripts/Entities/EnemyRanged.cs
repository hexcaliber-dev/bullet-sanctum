using System.Collections;
using UnityEngine;

// Basic ranged enemy that requires players to be on the same level to attack.
// Cannot go through walls.
public class EnemyRanged : Enemy {

    public float detectRange, xRange, yRange;
    public EnemyWeapon weapon;

    public float STEP_MAX = 5;
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

    // This will spam shoot bullets if put in update(). Use coroutines
    public override void Attack () {
        weapon.UseWeapon ();
    }

    // This is called once per update
    public override void PlayerLookout () {
        // print (player.transform.position.x - transform.position.x);
        if (Mathf.Abs (player.transform.position.x - transform.position.x) < xRange &&
            Mathf.Abs (player.transform.position.y - transform.position.y) < yRange) {
            playerFound = true;
            weapon.canShoot = true;
        } else {
            playerFound = false;
            weapon.canShoot = false;
        }
    }

    // This is called once per update
    public override void MovePattern () {
        // print (playerFound);
        if (playerFound) {
            rb.velocity = new Vector2 (0f, rb.velocity.y);
        } else {
            if (Vector2.Distance(player.transform.position, transform.position) < detectRange)
                rb.AddForce (Vector3.Normalize ((Vector2) (player.transform.position - transform.position)) * speed);
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
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag == "Player" && playerFound) {
            Vector2 knockback = new Vector2 (8, 3);
            if (facingRight) {
                knockback.x = -knockback.x;
            }
            rb.AddForce (knockback, ForceMode2D.Impulse);
        }
    }
}