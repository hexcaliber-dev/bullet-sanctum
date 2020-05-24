using System;
using System.Collections;
using UnityEngine;

public class Player : LivingEntity {

    const int STARTING_HEALTH = 20;

    public int maxHealth; // current health is inherited from LivingEntity

    public Weapon currWeapon;
    public GameObject trailObj, shoulder;
    public Color trailColor;
    public HUD hud;
    public Sprite playerSprite, crouchSprite, weaponSprite, weaponSpriteFlipped;

    // Basic movement variables
    public float accelStrength, decelMultiplier, jumpStrength, maxSpeed, deadzone, coyoteTime, crouchSpeed;

    // Strafing variables
    public float strafeStrength, strafeTime, strafeCooldownTime, strafeRechargeTime;
    public int strafeCost;
    public bool strafeCooldown;

    // Move state
    public enum MoveState { Ground, Falling, Jumping, Strafing }
    public MoveState currState;

    // Hidden variables
    const int MAX_STRAFE_BARS = 6;
    float currVelocity, currVelocityJump;
    int strafesRemaining;
    Rigidbody2D rb2D;
    BoxCollider2D thisCol;

    override protected void Start () {
        base.Start ();
        thisCol = GetComponent<BoxCollider2D> ();
        rb2D = GetComponent<Rigidbody2D> ();
        maxHealth = STARTING_HEALTH;
        health = STARTING_HEALTH;
        strafesRemaining = MAX_STRAFE_BARS;
        StartCoroutine (RechargeStrafe ());
    }

    void FixedUpdate () {
        bool decel = true; // True if player is decelerating

        // Get mouse direction
        Vector3 mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2 direction = mouse - transform.position;
        Vector2 strafeDir = Vector2.zero;

        // Rotate weapon to point at mouse
        Vector2 diff = Vector3.Normalize(direction);
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        shoulder.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        LayerMask groundMask = LayerMask.GetMask("Ground");
        // Check if player is grounded
        if (currState != MoveState.Strafing &&
            (Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y + 0.01f), Vector2.down, 0.1f, groundMask) ||
                Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y + 0.01f) + Vector3.left * (thisCol.bounds.extents.x), Vector2.down, 0.1f, groundMask) ||
                Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y + 0.01f) + Vector3.right * (thisCol.bounds.extents.x), Vector2.down, 0.1f, groundMask))) {
            currState = MoveState.Ground;
            // strafeCooldown = false;
        } else if (currState == MoveState.Ground) {
            StartCoroutine (CoyoteTime ());
        }

        // Flip player if mouse is pointed left
        if (direction.x < 0) {
            transform.rotation = Quaternion.Euler (0, 180, 0);
            currWeapon.GetComponent<SpriteRenderer>().sprite = weaponSpriteFlipped;
            currWeapon.transform.localRotation = Quaternion.Euler (0, 0, 0);
        } else {
            transform.rotation = Quaternion.identity;
            currWeapon.GetComponent<SpriteRenderer>().sprite = weaponSprite;
            currWeapon.transform.localRotation = Quaternion.identity;
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
            if (currState != MoveState.Strafing && !strafeCooldown && strafesRemaining >= strafeCost) {
                currState = MoveState.Strafing;
                if (strafeDir.Equals (Vector2.zero)) {
                    // Mouse pointer direction strafing
                    rb2D.velocity = new Vector2 (strafeStrength * ((direction.x < 0) ? -1 : 1), 0);
                } else {
                    rb2D.velocity = strafeStrength * Vector3.Normalize (strafeDir);
                }
                strafeCooldown = true;
                strafesRemaining -= strafeCost;
                hud.SetStrafeAmount (strafesRemaining);
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
            // Crouching
            if (Input.GetKey (KeyCode.LeftControl)) {
                rb2D.velocity = new Vector2 (Mathf.Clamp (currVelocity, -crouchSpeed, crouchSpeed), rb2D.velocity.y);
                GetComponent<SpriteRenderer>().sprite = crouchSprite;
            } else {
                rb2D.velocity = new Vector2 (Mathf.Clamp (currVelocity, -maxSpeed, maxSpeed), rb2D.velocity.y);
                GetComponent<SpriteRenderer>().sprite = playerSprite;
            }
        }

        // Shooting
        if (Input.GetKey (KeyCode.Mouse0)) {
            if (!currWeapon.onCooldown) {
                hud.UpdateRechargeMeter (currWeapon);
                currWeapon.UseWeapon ();
            }
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
            StartCoroutine (CreateTrail (0.15f, trailColor.a * (1f - (0.1f * i))));
            yield return new WaitForSeconds (seconds / 10);
        }
        if (currState == MoveState.Strafing) {
            currState = MoveState.Falling;
        }
        rb2D.inertia = 0f;
        rb2D.velocity = Vector2.zero;
        GetComponent<SpriteRenderer> ().color = Color.white;
        yield return new WaitForSeconds (strafeCooldownTime);
        strafeCooldown = false;
        // rb2D.velocity = Vector2.down * strafeFallStrength;
    }

    IEnumerator CoyoteTime () {
        yield return new WaitForSeconds (coyoteTime);
        currState = MoveState.Falling;
    }

    IEnumerator CreateTrail (float duration, float alpha) {
        GameObject trail = GameObject.Instantiate (trailObj, transform.position, transform.rotation);
        trail.GetComponent<SpriteRenderer> ().color = new Color (trailColor.r, trailColor.g, trailColor.b, alpha);
        yield return new WaitForSeconds (duration);
        Destroy (trail);
    }

    IEnumerator RechargeStrafe () {
        while (true) {
            if (currState == MoveState.Ground) {
                strafesRemaining = Math.Min (MAX_STRAFE_BARS, strafesRemaining + 1);
                hud.SetStrafeAmount (strafesRemaining);
            }
            yield return new WaitForSeconds (strafeRechargeTime);
        }
    }
}