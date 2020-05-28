using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/** Handles movement and animation of player.
 * For shooting, go to PlayerShoot.
 * For Player HUD, go to HUD.
 * For health and bounty systems, go to PlayerStats.
 */
public class Player : LivingEntity {

    public bool doPlayerUpdates;
    const int STARTING_HEALTH = 20;
    public int maxHealth; // current health is inherited from LivingEntity

    public GameObject trailObj, shoulder, arm;
    public Color trailColor;
    public Sprite playerSprite, crouchSprite, weaponSprite, weaponSpriteFlipped;

    // Basic movement variables
    public float accelStrength, decelMultiplier, jumpStrength, jumpTime, maxSpeed, deadzone, coyoteTime, crouchSpeed;

    // Strafing variables
    public float strafeStrength, strafeTime, strafeCooldownTime, strafeRechargeTime;
    public int strafeCost;
    public bool strafeCooldown;
    public float bulletTimeDistance, bulletTimeMultiplier;

    // Move state
    public enum MoveState { Ground, Falling, Jumping, Strafing }
    public MoveState currState;

    // Hidden variables
    const int MAX_STRAFE_BARS = 6;
    float currVelocity;
    int strafesRemaining;
    Rigidbody2D rb2D;
    BoxCollider2D thisCol;
    Vector3 respawnPoint;
    HUD hud;

    Animator animator;

    override protected void Start () {
        base.Start ();
        hud = GameObject.FindObjectOfType<HUD>();
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
                arm.GetComponent<SpriteRenderer> ().sprite = weaponSpriteFlipped;
                arm.transform.localRotation = Quaternion.Euler (0, 0, 0);
                animator.SetFloat ("walkSpeed", -1f);
            } else {
                transform.rotation = Quaternion.identity;
                arm.GetComponent<SpriteRenderer> ().sprite = weaponSprite;
                arm.transform.localRotation = Quaternion.identity;
                animator.SetFloat ("walkSpeed", 1f);
            }

            // Horizontal movement
            float horizMovement = (Input.GetKey (KeyCode.A) ? -1 : (Input.GetKey (KeyCode.D) ? 1 : 0));

            if (horizMovement != 0 && currState == MoveState.Ground)
                animator.SetBool ("moving", true);

            // print ("CV " + currVelocity + " HM " + horizMovement);
            if ((currVelocity <= 0 && horizMovement < 0) || (currVelocity >= 0 && horizMovement > 0)) {
                if (Mathf.Abs (currVelocity) <= ((animator.GetBool ("crouching")) ? crouchSpeed : maxSpeed)) {
                    currVelocity = Mathf.Min (Mathf.Abs (currVelocity) + accelStrength * Time.deltaTime, ((animator.GetBool ("crouching")) ? crouchSpeed : maxSpeed)) * horizMovement;
                    decel = false;
                    print(currVelocity);
                } else {
                    decel = true;
                }
            } else {
                decel = true;
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
        }

        // Old debug print statements
        // print (currVelocity);
        // print (currState);

        // Velocity handling for horizontal movement
        if (currState != MoveState.Strafing) {
            // Crouching
            if (Input.GetKey (KeyCode.LeftControl)) {
                animator.SetBool ("crouching", true);
                GetComponent<SpriteRenderer> ().sprite = crouchSprite;
                shoulder.transform.localPosition = new Vector2 (-0.03f, 0f);
                GetComponent<BoxCollider2D> ().size = new Vector2 (.18f, .3f);
            } else {
                shoulder.transform.localPosition = new Vector2 (-0.03f, 0.04f);

                if (!hittingCeiling) {
                    animator.SetBool ("crouching", false);
                    GetComponent<SpriteRenderer> ().sprite = playerSprite;
                    GetComponent<BoxCollider2D> ().size = new Vector2 (.18f, .4f);
                }
            }
        }

        // Deceleration mechanic
        if (decel) {
            currVelocity = rb2D.velocity.x;
            currVelocity *= decelMultiplier;
            if (Math.Abs (rb2D.velocity.x) < deadzone) {
                currVelocity = 0f;
            }
            rb2D.velocity = new Vector2 (currVelocity, rb2D.velocity.y);
            animator.SetBool ("moving", false);
        } else {
            if (Math.Abs (currVelocity) > Math.Abs (rb2D.velocity.x))
                rb2D.velocity = new Vector2 (currVelocity, rb2D.velocity.y);
        }
    }

    public override void OnSpawn () {
        // Do nothing?
    }

    public override void OnDeath () {
        // TODO move player to last checkpoint and reset bounty
    }

    public override void Attack () {
        // Not implemented. Go to PlayerShoot for player attack behavior
    }

    public override void TakeDamage (Bullet b) {
        TakeDamage (b.damage);
    }

    public override void TakeDamage (int damage) {
        base.TakeDamage (damage);
        GameObject.FindObjectOfType<CameraUtils> ().Shake ();
        hud.SetHealthAmount (health);
    }

    IEnumerator StartStrafe (float delay, bool inverted) {
        currState = MoveState.Strafing;
        strafeCooldown = true;
        strafesRemaining -= strafeCost;
        Vector2 strafeDir = Vector2.zero;
        hud.SetStrafeAmount (strafesRemaining);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Enemy"), true);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("EnemyBullet"), true);

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag ("Enemy")) {
            print (Vector2.Distance (enemy.transform.position, transform.position));
            if (Vector2.Distance (enemy.transform.position, transform.position) < bulletTimeDistance) {
                Time.timeScale = bulletTimeMultiplier;
                print (Time.timeScale);
            }
        }

        GetComponent<SpriteRenderer> ().color = new Color (1, 1, .62f, 0.25f); // temp transparency effect
        yield return new WaitForSeconds (delay);
        if (Input.GetKey (KeyCode.W)) strafeDir += Vector2.up;
        if (Input.GetKey (KeyCode.A)) strafeDir += Vector2.left;
        if (Input.GetKey (KeyCode.S)) strafeDir += Vector2.down;
        if (Input.GetKey (KeyCode.D)) strafeDir += Vector2.right;

        if (!strafeDir.Equals (Vector2.zero)) {
            rb2D.velocity = strafeStrength * Vector3.Normalize (strafeDir);
        }
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
        yield return new WaitForSeconds (strafeCooldownTime);
        strafeCooldown = false;
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Enemy"), false);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("EnemyBullet"), false);
        Time.timeScale = 1f;
        GetComponent<SpriteRenderer> ().color = Color.white;
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

    // Called when player goes out of bounds or gets killed by elemental hazards.
    public void Respawn () {
        // TODO death screen or something
        rb2D.velocity = Vector2.zero;
        transform.position = respawnPoint;
    }

    // Picks up a weapon from the ground.
    public void CollectWeapon (Weapon weapon) {

    }

}