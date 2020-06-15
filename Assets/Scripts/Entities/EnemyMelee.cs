using System;
using UnityEngine;

public class EnemyMelee : Enemy {

    public float STEP_MAX = 5;
    private bool movingRight;
    private float startPos;
    private float endPos;

    public override void OnSpawn () {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        enemyType = EnemyType.Melee;
        PlayerLookout ();
        movingRight = facingRight;
        startPos = transform.position.x;
        endPos = startPos + STEP_MAX;
        GetComponent<SpriteRenderer> ().flipX = true;
        GetComponent<SpriteRenderer> ().size = new Vector2 (3f, 3f);
    }

    public override void Attack () {
        player.TakeDamage (DMG);
    }

    // This is inherited from LivingEntity
    // public override void OnDeath() {
    //     // Fucking die.
    // }

    public override void MovePattern () {
        GetComponent<Animator> ().SetBool ("moving", playerFound);
        if (playerFound) {
            var direction = Vector3.zero;
            if (Vector3.Distance (transform.position, player.transform.position) > 0.1) {
                direction = player.transform.position - transform.position;
                rb.AddRelativeForce (direction.normalized * speed, ForceMode2D.Force);
            }
        } else {
            if (movingRight) {
                rb.AddRelativeForce (new Vector2 (Vector2.right.x, rb.velocity.y), ForceMode2D.Force);
                if (!facingRight) {
                    Flip ();
                }
            }

            if (rb.position.x >= endPos) {
                movingRight = false;
                rb.velocity = Vector2.zero;
            }

            if (!movingRight) {
                rb.AddRelativeForce (new Vector2 (-Vector2.right.x, rb.velocity.y), ForceMode2D.Force);
                if (facingRight) {
                    Flip ();
                }
            }

            if (rb.position.x <= startPos) {
                movingRight = true;
                rb.velocity = Vector2.zero;
            }
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag == "Player" && playerFound) {
            AudioHelper.PlaySound ("vorpal_hurt3");
            Vector2 knockback = new Vector2 (8, 3);
            if (facingRight) {
                knockback.x = -knockback.x;
            }
            rb.AddForce (knockback, ForceMode2D.Impulse);
            Attack ();
        }
        if (collision.gameObject.tag == "Bullet") {
            AudioHelper.PlaySound ("vorpal_hurt");
            startPos = transform.position.x;
            endPos = startPos + STEP_MAX;
        }
    }

    public override void OnDeath () {
        AudioHelper.PlaySound ("vorpal_death");
        base.OnDeath ();
    }
}