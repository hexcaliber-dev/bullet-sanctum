using System;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class Player : LivingEntity {

    public bool doPlayerUpdates;
    const int STARTING_HEALTH = 20;
    public int maxHealth; // current health is inherited from LivingEntity

    public Weapon currWeapon;
    public GameObject trailObj, shoulder;
    public Color trailColor;
    public HUD hud;
    public Sprite playerSprite, crouchSprite, weaponSprite, weaponSpriteFlipped;

    // Basic movement variables
    public float accelStrength, decelMultiplier, jumpStrength, jumpTime, maxSpeed, deadzone, coyoteTime, crouchSpeed;

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
    Vector3 respawnPoint;

    Animator animator;

    override protected void Start () {
        base.Start ();
        thisCol = GetComponent<BoxCollider2D> ();
        rb2D = GetComponent<Rigidbody2D> ();
        maxHealth = STARTING_HEALTH;
        health = STARTING_HEALTH;
        strafesRemaining = MAX_STRAFE_BARS;
        StartCoroutine (RechargeStrafe ());
        animator = GetComponent<Animator> ();
        doPlayerUpdates = true;
    }

    void FixedUpdate () {
        bool decel = true; // True if player is decelerating

        // Get mouse direction
        Vector3 mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector2 direction = mouse - transform.position;

        // Rotate weapon to point at mouse
        Vector2 diff = Vector3.Normalize (direction);
        float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
        shoulder.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);

        LayerMask groundMask = LayerMask.GetMask ("Ground");

        // Ceiling detection
        bool hittingCeiling = Physics2D.Raycast (transform.position + Vector3.up * (thisCol.bounds.extents.y + 0.01f), Vector2.up, 0.1f, groundMask);

        bool centerRaycast = Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y + 0.01f), Vector2.down, 0.1f, groundMask);
        // Check if player is grounded
        if (currState != MoveState.Strafing &&
            (centerRaycast ||
                Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y + 0.01f) + Vector3.left * (thisCol.bounds.extents.x), Vector2.down, 0.1f, groundMask) ||
                Physics2D.Raycast (transform.position + Vector3.down * (thisCol.bounds.extents.y + 0.01f) + Vector3.right * (thisCol.bounds.extents.x), Vector2.down, 0.1f, groundMask))) {
            currState = MoveState.Ground;
            animator.SetInteger ("jumpState", 0);
        } else if (currState == MoveState.Ground) {
            StartCoroutine (CoyoteTime ());
        }

        // Set last valid respawn point
        if (centerRaycast) {
            respawnPoint = transform.position;
        }

        // Disable player updates when in menu, died, etc
        if (doPlayerUpdates) {
            // Flip player if mouse is pointed left
            if (direction.x < 0) {
                transform.rotation = Quaternion.Euler (0, 180, 0);
                currWeapon.GetComponent<SpriteRenderer> ().sprite = weaponSpriteFlipped;
                currWeapon.transform.localRotation = Quaternion.Euler (0, 0, 0);
                animator.SetFloat ("walkSpeed", -1f);
            } else {
                transform.rotation = Quaternion.identity;
                currWeapon.GetComponent<SpriteRenderer> ().sprite = weaponSprite;
                currWeapon.transform.localRotation = Quaternion.identity;
                animator.SetFloat ("walkSpeed", 1f);
            }

            // Moving left
            if (Input.GetKey (KeyCode.A)) {
                animator.SetBool ("moving", true);
                if (currVelocity <= 0) {
                    currVelocity -= accelStrength * Time.deltaTime;
                    decel = false;
                }
            }
            // Moving right
            if (Input.GetKey (KeyCode.D)) {
                animator.SetBool ("moving", true);
                if (currVelocity >= 0) {
                    currVelocity += accelStrength * Time.deltaTime;
                    decel = false;
                }
            }

            // Jumping
            if (Input.GetKey (KeyCode.Space)) {
                if (currState == MoveState.Ground && !hittingCeiling) {
                    StartCoroutine (Jump ());
                }
            }
            // Strafe
            if (Input.GetKey (KeyCode.LeftShift)) {
                if (currState != MoveState.Strafing && !strafeCooldown && strafesRemaining >= strafeCost) {
                    StartCoroutine (StartStrafe (0.025f, direction.x < 0));
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

        // Old debug print statements
        // print (currVelocity);
        // print (currState);

        // Velocity handling for horizontal movement
        if (currState != MoveState.Strafing) {
            // Crouching
            if (Input.GetKey (KeyCode.LeftControl)) {
                animator.SetBool ("crouching", true);
                rb2D.velocity = new Vector2 (Mathf.Clamp (currVelocity, -crouchSpeed, crouchSpeed), rb2D.velocity.y);
                GetComponent<SpriteRenderer> ().sprite = crouchSprite;
                shoulder.transform.localPosition = new Vector2 (-0.03f, 0f);
                GetComponent<BoxCollider2D> ().size = new Vector2 (.18f, .3f);
            } else {
                animator.SetBool ("crouching", false);
                rb2D.velocity = new Vector2 (Mathf.Clamp (currVelocity, -maxSpeed, maxSpeed), rb2D.velocity.y);
                shoulder.transform.localPosition = new Vector2 (-0.03f, 0.04f);

                if (!hittingCeiling) {
                    GetComponent<SpriteRenderer> ().sprite = playerSprite;
                    GetComponent<BoxCollider2D> ().size = new Vector2 (.18f, .4f);
                }
            }
        }

        // Deceleration mechanic
        if (decel) {
            currVelocity *= decelMultiplier;
            if (Math.Abs (currVelocity) < deadzone) {
                currVelocity = 0f;
            }
            animator.SetBool ("moving", false);
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

    IEnumerator StartStrafe (float delay, bool inverted) {
        currState = MoveState.Strafing;
        strafeCooldown = true;
        strafesRemaining -= strafeCost;
        Vector2 strafeDir = Vector2.zero;
        hud.SetStrafeAmount (strafesRemaining);

        yield return new WaitForSeconds (delay);
        if (Input.GetKey (KeyCode.W)) strafeDir += Vector2.up;
        if (Input.GetKey (KeyCode.A)) strafeDir += Vector2.left;
        if (Input.GetKey (KeyCode.S)) strafeDir += Vector2.down;
        if (Input.GetKey (KeyCode.D)) strafeDir += Vector2.right;

        if (!strafeDir.Equals (Vector2.zero)) {
            rb2D.velocity = strafeStrength * Vector3.Normalize (strafeDir);
        }
        GetComponent<SpriteRenderer> ().color = new Color (1, 1, .62f, 0.25f); // temp transparency effect
        StartCoroutine (StopStrafe (strafeTime));
    }

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
        if (currState != MoveState.Jumping) {
            currState = MoveState.Falling;
        }
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

    IEnumerator Jump () {
        animator.SetInteger ("jumpState", 1);
        const int RESOLUTION = 10;
        currState = MoveState.Jumping;
        for (int i = 0; i < RESOLUTION && Input.GetKey (KeyCode.Space); i += 1) {
            rb2D.velocity = new Vector2 (rb2D.velocity.x, jumpStrength);
            yield return new WaitForSeconds (jumpTime / RESOLUTION);
        }
        currState = MoveState.Falling;
        animator.SetInteger ("jumpState", 2);
    }

    void OnTriggerExit2D (Collider2D col) {
        // Player fell out of the world
        if (col.gameObject.layer == LayerMask.NameToLayer ("BoundingBox")) {
            print ("RESPAWN");
            Respawn ();
        }
    }

    public void Respawn () {
        // TODO death screen or something
        rb2D.velocity = Vector2.zero;
        transform.position = respawnPoint;
    }
}