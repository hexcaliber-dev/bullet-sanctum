using System;
using System.Collections;
using UnityEngine;

public class Player : LivingEntity {

    const int STARTING_HEALTH = 20;

    public int maxHealth; // current health is inherited from LivingEntity

    public Weapon currWeapon;
    public GameObject trailObj;
    public Color trailColor;

    Rigidbody2D rb2D;

    BoxCollider2D thisCol;

    public float accelStrength, decelMultiplier, jumpStrength, strafeStrength, strafeFallStrength, maxSpeed, deadzone, strafeTime, coyoteTime; // assign in inspector
    public bool strafeCooldown;

    public enum MoveState { Ground, Falling, Jumping, Strafing }
    public MoveState currState;

    float currVelocity, currVelocityJump;

    override protected void Start () {
        base.Start ();
        thisCol = GetComponent<BoxCollider2D> ();
        rb2D = GetComponent<Rigidbody2D> ();
        maxHealth = STARTING_HEALTH;
        health = STARTING_HEALTH;
    }

    void FixedUpdate () {
        bool decel = true; // True if player is decelerating

        // Get mouse direction
        Vector3 mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2 direction = mouse - transform.position;
        Vector2 strafeDir = Vector2.zero;

        // Check if player is grounded
        if (Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y), Vector2.down, 0.1f) ||
            Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y) + Vector3.left * (thisCol.bounds.extents.x), Vector2.down, 0.1f) ||
            Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y) + Vector3.right * (thisCol.bounds.extents.x), Vector2.down, 0.1f)) {
            // print ("GROUNDED");
            currState = MoveState.Ground;
            strafeCooldown = false;
        } else if (currState == MoveState.Ground) {
            StartCoroutine (CoyoteTime ());
        }

        // Flip player if mouse is pointed left
        if (direction.x < 0) {
            transform.rotation = Quaternion.Euler (0, 180, 0);
        } else {
            transform.rotation = Quaternion.identity;
        }

        // Moving left
        if (Input.GetKey (KeyCode.A)) {
            if (currVelocity <= 0) {
                currVelocity -= accelStrength * Time.deltaTime;
                decel = false;
            }
            strafeDir += Vector2.left;
        }
        // Moving right
        if (Input.GetKey (KeyCode.D)) {
            if (currVelocity >= 0) {
                currVelocity += accelStrength * Time.deltaTime;
                decel = false;
            }
            strafeDir += Vector2.right;
        }

        if (Input.GetKey (KeyCode.W)) {
            strafeDir += Vector2.up;
        }
        if (Input.GetKey (KeyCode.S)) {
            strafeDir += Vector2.down;
        }

        // Jumping
        if (Input.GetKey (KeyCode.Space)) {
            if (currState == MoveState.Ground) {
                //currVelocityJump = 10f; //starting velocity going up, delete if not needed
                rb2D.velocity = jumpStrength * Vector2.up;
                currState = MoveState.Jumping;
            }
        }
        // Strafe
        if (Input.GetKey (KeyCode.LeftShift)) {
            if (currState != MoveState.Strafing && !strafeCooldown) {
                currState = MoveState.Strafing;
                if (strafeDir.Equals (Vector2.zero)) {
                    // Mouse pointer direction strafing
                    rb2D.velocity = new Vector2 (strafeStrength * ((direction.x < 0) ? -1 : 1), 0);
                } else {
                    rb2D.velocity = strafeStrength * Vector3.Normalize (strafeDir);
                }
                strafeCooldown = true;
                GetComponent<SpriteRenderer> ().color = new Color (1, 1, .62f, 0.25f); // temp transparency effect
                StartCoroutine (StopStrafe (strafeTime));
            }
        }
        // Deceleration mechanic
        if (decel) {
            currVelocity *= decelMultiplier;
            if (Math.Abs (currVelocity) < deadzone) {
                currVelocity = 0f;
            }
        }

        // Old debug print statements
        // print (currVelocity);
        // print (currState);

        // Velocity handling for horizontal movement
        if (currState != MoveState.Strafing) {
            rb2D.velocity = new Vector2 (Mathf.Clamp (currVelocity, -maxSpeed, maxSpeed), rb2D.velocity.y);
        }

        // Shooting
        if (Input.GetKey (KeyCode.Mouse0)) {
            currWeapon.UseWeapon ();
        }
    }

    public override void OnSpawn () {

    }

    public override void OnDeath () {

    }

    public override void Attack () {
        currWeapon.UseWeapon ();
    }

    public override void TakeDamage (Bullet b) {

    }

    // Collision is handled in PlayerFeet

    IEnumerator StopStrafe (float seconds) {
        for (int i = 0; i < 5; i += 1) {
            StartCoroutine(CreateTrail(0.15f, trailColor.a * (1f - (0.1f * i))));
            yield return new WaitForSeconds(seconds / 10);
        }
        if (currState == MoveState.Strafing) {
            currState = MoveState.Falling;
        }
        rb2D.inertia = 0f;
        rb2D.velocity = Vector2.zero;
        GetComponent<SpriteRenderer> ().color = Color.white;
        // rb2D.velocity = Vector2.down * strafeFallStrength;
    }

    IEnumerator CoyoteTime () {
        yield return new WaitForSeconds (coyoteTime);
        currState = MoveState.Falling;
    }

    IEnumerator CreateTrail (float duration, float alpha) {
        GameObject trail = GameObject.Instantiate(trailObj, transform.position, transform.rotation);
        trail.GetComponent<SpriteRenderer>().color = new Color(trailColor.r, trailColor.g, trailColor.b, alpha);    
        yield return new WaitForSeconds(duration);
        Destroy(trail);
    }
}