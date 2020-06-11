using System.Collections;
using UnityEngine;

public class EnemyWarden : Enemy {

    public float STEP_MAX = 5;
    private bool movingRight;
    private float startPos;
    private float endPos;
    public float flipDelay, chargeDelay, chargeSpeed, chargeRange, chargeTime;
    bool canCharge, isCharging;

    public override void OnSpawn () {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        enemyType = EnemyType.Melee;
        PlayerLookout ();
        movingRight = facingRight;
        startPos = transform.position.x;
        endPos = startPos + STEP_MAX;
        GetComponent<SpriteRenderer> ().size = new Vector2 (3f, 3f);
        canCharge = true;
    }

    public override void Attack () {
        player.TakeDamage (DMG);
    }

    // This is inherited from LivingEntity
    // public override void OnDeath() {
    //     // Fucking die.
    // }

    public override void MovePattern () {
        if (!isCharging) {
            GetComponent<Animator> ().SetBool ("moving", playerFound);
            if (canCharge &&
                Mathf.Abs (transform.position.x - player.transform.position.x) < chargeRange &&
                Mathf.Abs (transform.position.y - player.transform.position.y) < 0.5f) {
                StartCoroutine (cCharge ());
            }
        }
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
                    StartCoroutine (cFlip ());
                }
            }

            if (rb.position.x >= endPos) {
                movingRight = false;
                rb.velocity = Vector2.zero;
            }

            if (!movingRight) {
                rb.AddRelativeForce (new Vector2 (-Vector2.right.x, rb.velocity.y), ForceMode2D.Force);
                if (facingRight) {
                    StartCoroutine (cFlip ());
                }
            }

            if (rb.position.x <= startPos) {
                movingRight = true;
                rb.velocity = Vector2.zero;
            }
        }
    }

    IEnumerator cFlip () {
        yield return new WaitForSeconds (flipDelay);
        if (!isCharging) {
            GetComponent<SpriteRenderer> ().flipX = !movingRight;
        }
    }

    IEnumerator cCharge () {
        rb.velocity = Vector2.zero;
        GetComponent<Animator> ().SetBool ("moving", false);
        canCharge = false;
        isCharging = true;
        yield return new WaitForSeconds (chargeDelay);
        GetComponent<Animator> ().SetBool ("moving", true);
        float oldSpeed = speed;
        speed = chargeSpeed;
        print ("CHARGE" + speed);
        yield return new WaitForSeconds (chargeTime);
        speed = oldSpeed;
        isCharging = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds (chargeDelay);
        canCharge = true;
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag == "Player" && playerFound) {
            Vector2 knockback = new Vector2 (8, 3);
            if (facingRight) {
                knockback.x = -knockback.x;
            }
            rb.AddForce (knockback, ForceMode2D.Impulse);
            Attack ();
        }
        if (collision.gameObject.tag == "Bullet") {
            startPos = transform.position.x;
            endPos = startPos + STEP_MAX;
        }
    }
}