using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Handles movement and animation of player.
 * For shooting, go to PlayerShoot.
 * For Player HUD, go to HUD.
 * For health and bounty systems, go to PlayerStats.
 */
public class Player : LivingEntity {

    public bool doPlayerUpdates;
    const int STARTING_HEALTH = 20;
    public static int maxHealth = STARTING_HEALTH;
    public static int playerHealth = STARTING_HEALTH;
    public static int potions = 0;

    public GameObject trailObj, shoulder, arm;
    public Color trailColor;
    public Sprite playerSprite, crouchSprite, weaponSprite, weaponSpriteFlipped;

    // Basic movement variables
    public float accelStrength, decelMultiplier, jumpStrength, jumpTime, maxSpeed, deadzone, coyoteTime, crouchSpeed, bulletTime;

    // Strafing variables
    public float strafeStrength, strafeTime, strafeCooldownTime, strafeRechargeTime;
    public int strafeCost;
    public bool strafeCooldown;
    public float bulletTimeDistance, bulletTimeMultiplier;

    // Invulnerability
    bool canTakeDamage;
    public float invulnerabilityTime;

    // Potions
    public int potionHealAmount;

    // Move state
    public enum MoveState { Ground, Falling, Jumping, Strafing }
    public MoveState currState;

    // Hidden variables
    const int MAX_STRAFE_BARS = 6;
    int strafesRemaining;
    Rigidbody2D rb2D;
    BoxCollider2D thisCol;
    Vector3 respawnPoint;
    HUD hud;

    Animator animator;

    override protected void Start () {
        base.Start ();
        hud = GameObject.FindObjectOfType<HUD> ();
        thisCol = GetComponent<BoxCollider2D> ();
        rb2D = GetComponent<Rigidbody2D> ();
        strafesRemaining = MAX_STRAFE_BARS;
        StartCoroutine (RechargeStrafe ());
        animator = GetComponent<Animator> ();
        doPlayerUpdates = true;
        hud.SetHealthAmount (playerHealth);
        hud.SetScrollAmount((Shop.hasScroll) ? 1 : 0);
        hud.SetHealthPotions(potions);
        Time.timeScale = 1f;
        canTakeDamage = true;
    }

    void FixedUpdate () {
        bool decel = false; // True if player is decelerating
        float newVX = rb2D.velocity.x; // New horizontal velocity value after movement calculations

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
            if (animator.GetInteger ("jumpState") != 0) {
                AudioHelper.PlaySound ("landing", 0.6f);
                animator.SetInteger ("jumpState", 0);
            }
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

            // No key was pressed - slow down player
            if (horizMovement == 0)
                decel = true;

            // Stuff for animation
            if (horizMovement != 0 && currState == MoveState.Ground)
                animator.SetBool ("moving", true);

            // Adding velocity to player
            if (Mathf.Sign (newVX) == Mathf.Sign (horizMovement) || Math.Abs (newVX) < deadzone) {
                if (Mathf.Abs (newVX) <= ((animator.GetBool ("crouching")) ? crouchSpeed : maxSpeed))
                    newVX = Mathf.Min (Mathf.Abs (newVX) + accelStrength * Time.deltaTime, ((animator.GetBool ("crouching")) ? crouchSpeed : maxSpeed)) * horizMovement;
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

            // Crouching
            if (currState != MoveState.Strafing) {
                if (Input.GetKey (KeyCode.S)) {
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

            // Potion consumption
            if (potions > 0 && Input.GetKeyDown (KeyCode.H)) {
                potions -= 1;
                hud.SetHealthPotions (potions);
                playerHealth = Mathf.Clamp (playerHealth + potionHealAmount, 0, maxHealth);
                AudioHelper.PlaySound ("potion");
                hud.SetHealthAmount (playerHealth);
            }
        }
        AudioHelper.SetWalking (animator.GetBool ("moving"));

        // Deceleration mechanic; apply changes to velocity
        if (currState != MoveState.Strafing) {
            if (Math.Abs (newVX) < Math.Abs (rb2D.velocity.x) || decel || !doPlayerUpdates) {
                newVX = rb2D.velocity.x;
                newVX *= decelMultiplier;
                animator.SetBool ("moving", false);
            }
            if (Math.Abs (newVX) < deadzone) {
                newVX = 0f;
            }
            rb2D.velocity = new Vector2 (newVX, rb2D.velocity.y);
        }
    }

    public override void OnSpawn () {
        // Do nothing?
    }

    public override void OnDeath () {
        // TODO move player to last checkpoint and reset bounty
        GetComponent<PlayerBounty> ().ResetBounty ();
        if (Checkpoint.getCurrentCheckpoint ().Equals ("")) {
            RoomSwitch.currentId = "1";
            SceneManager.LoadScene ("0_CaveOfRebirth");
        } else {
            RoomSwitch.currentId = "99";
            SceneManager.LoadScene (Checkpoint.getCurrentCheckpoint ());
        }
        playerHealth = maxHealth;
    }

    public override void Attack () {
        // Not implemented. Go to PlayerShoot for player attack behavior
    }

    public override void TakeDamage (Bullet b) {
        if (playerHealth > 0) {
            TakeDamage (b.damage);
        }
    }

    public override void TakeDamage (int damage) {
        if (canTakeDamage && currState != MoveState.Strafing) {
            StartCoroutine (FlashWhite (0.1f));
            StartCoroutine (Invulnerability ());
            playerHealth -= damage;
            if (playerHealth <= 0) {
                OnDeath ();
            } else {
                if (playerHealth > maxHealth) {
                    playerHealth = maxHealth;
                } else {
                    AudioHelper.PlaySound ("playerhurt_alt", 0.3f);
                    GameObject.FindObjectOfType<CameraUtils> ().Shake (0.25f, 0.25f);
                }
                hud.SetHealthAmount (playerHealth);
            }
        }
    }

    IEnumerator Invulnerability () {
        canTakeDamage = false;
        yield return new WaitForSeconds (invulnerabilityTime);
        canTakeDamage = true;
    }

    IEnumerator StartStrafe (float delay, bool inverted) {
        bool doBulletTime = false;
        currState = MoveState.Strafing;
        strafeCooldown = true;
        strafesRemaining -= strafeCost;
        Vector2 strafeDir = Vector2.zero;
        hud.SetStrafeAmount (strafesRemaining);
        AudioHelper.PlaySound ("dash");
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Enemy"), true);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Ghost"), true);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("EnemyBullet"), true);

        GetComponent<SpriteRenderer> ().color = new Color (1, 1, .62f, 0.25f); // temp transparency effect
        yield return new WaitForSeconds (delay);
        if (Input.GetKey (KeyCode.W)) strafeDir += Vector2.up;
        if (Input.GetKey (KeyCode.A)) strafeDir += Vector2.left;
        if (Input.GetKey (KeyCode.S)) strafeDir += Vector2.down;
        if (Input.GetKey (KeyCode.D)) strafeDir += Vector2.right;

        List<GameObject> bulletTimeObjs = new List<GameObject> ();
        bulletTimeObjs.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
        bulletTimeObjs.AddRange (GameObject.FindGameObjectsWithTag ("EnemyBullet"));
        foreach (GameObject enemy in bulletTimeObjs) {
            // Bullet time
            if (Vector2.Distance (enemy.transform.position, transform.position) < bulletTimeDistance && !doBulletTime) {
                Time.timeScale = bulletTimeMultiplier;
                StartCoroutine (hud.DoBulletTime (bulletTime));
                AudioHelper.PlaySound ("bullettime");
                doBulletTime = true;
            }
        }
        if (!strafeDir.Equals (Vector2.zero)) {
            rb2D.velocity = ((doBulletTime) ? strafeStrength / 3f : strafeStrength) * Vector3.Normalize (strafeDir);
        }
        StartCoroutine ((doBulletTime) ? StopStrafe (bulletTime) : StopStrafe (strafeTime));
    }

    IEnumerator StopStrafe (float seconds) {
        for (int i = 0; i < 5; i += 1) {
            StartCoroutine (CreateTrail (0.15f, trailColor.a * (1f - (0.1f * i))));
            yield return new WaitForSeconds (seconds / 5);
        }
        if (currState == MoveState.Strafing) {
            currState = MoveState.Falling;
        }
        rb2D.inertia *= 0.25f;
        rb2D.velocity = Vector2.zero;
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Enemy"), false);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Ghost"), false);
        Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("EnemyBullet"), false);
        yield return new WaitForSeconds (strafeCooldownTime);
        strafeCooldown = false;
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
                // if (strafesRemaining == 3) {
                //     AudioHelper.PlaySound ("recharged");
                // }
            }
            yield return new WaitForSeconds (strafeRechargeTime);
        }
    }

    IEnumerator Jump () {
        animator.SetInteger ("jumpState", 1);
        const int RESOLUTION = 10;
        currState = MoveState.Jumping;
        AudioHelper.PlaySound ("jump");
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
    public IEnumerator Respawn () {
        hud.fadeImage.color = Color.red;
        yield return new WaitForSeconds (0.25f);
        // TODO death screen or something
        rb2D.velocity = Vector2.zero;
        transform.position = respawnPoint;
        for (float i = hud.fadeImage.color.a * 255; i > 0; i -= 5) {
            hud.fadeImage.color = new Color (0, 0, 0, i / 255);
            yield return null;
        }
    }

    public void ResetVelocity () {
        rb2D.velocity = Vector2.zero;
        rb2D.AddForce (Vector2.up);
    }

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.layer == LayerMask.NameToLayer ("Lava")) {
            AudioHelper.PlaySound ("playerhurt_alt");
            TakeDamage (5);
            StartCoroutine (Respawn ());
        }
    }

    public void GetPotion () {
        potions += 1;
        hud.SetHealthPotions (potions);
        AudioHelper.PlaySound ("checkpoint");
    }
}