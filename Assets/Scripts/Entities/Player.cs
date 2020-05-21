using System;
using System.Collections;
using UnityEngine;

public class Player : LivingEntity {

    const int STARTING_HEALTH = 20;

    public int maxHealth; // current health is inherited from LivingEntity

    public Weapon currWeapon;

    Rigidbody2D rb2D;

    public float accelStrength, decelMultiplier, jumpStrength, strafeStrength, strafeFallStrength, maxSpeed, deadzone; // assign in inspector
    bool strafeCooldown;

    enum MoveState { Ground, Falling, Jumping, Strafing }
    MoveState currState;

    float currVelocity, currVelocityJump;

    override protected void Start () {
        base.Start();
        rb2D = GetComponent<Rigidbody2D> ();
        maxHealth = STARTING_HEALTH;
        health = STARTING_HEALTH;
    }

    void FixedUpdate () {
        bool decel = true; // True if player is decelerating

        // Get mouse direction
        Vector3 mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2 direction = Vector3.Normalize (mouse - transform.position);

        // Flip player if mouse is pointed left
        if (direction.x < 0) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        } else {
            transform.rotation = Quaternion.identity;
        }

        // Moving left
        if (Input.GetKey (KeyCode.A)) {
            if (currVelocity <= 0) {
                currVelocity -= accelStrength * Time.deltaTime;
                decel = false;
            }
        }
        // Moving right
        if (Input.GetKey (KeyCode.D)) {
            if (currVelocity >= 0) {
                currVelocity += accelStrength * Time.deltaTime;
                decel = false;
            }
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
                rb2D.velocity = strafeStrength * direction;
                strafeCooldown = true;
                StartCoroutine (StopStrafe (0.1f));
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
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            currWeapon.UseWeapon();
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

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag.Equals ("Ground")) {
            currState = MoveState.Ground;
            strafeCooldown = false;
        }
    }

    void OnCollisionExit2D (Collision2D col) {
        if (col.gameObject.tag.Equals ("Ground")) {
            StartCoroutine (coyoteTime (0.25f));
        }
    }

    IEnumerator coyoteTime (float seconds) {
        // yield return new WaitForSeconds (seconds);
        // currState = MoveState.Falling;
        yield return null;
    }

    IEnumerator StopStrafe (float seconds) {
        yield return new WaitForSeconds (seconds);
        currState = MoveState.Falling;
        rb2D.inertia = 0f;
        rb2D.velocity = Vector2.zero;
        // rb2D.velocity = Vector2.down * strafeFallStrength;
    }
}