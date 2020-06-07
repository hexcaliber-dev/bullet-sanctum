using System.Collections;
using UnityEngine;

public class EnemyFracture : Enemy {

    private int shockCounter;

    // 0 = Not decided, 1 = Ground attk, 2 = Air attk, 3 = Shock attk.
    private int attackPattern;
    private float attackCooldown;
    public float baseCooldown;
    public Weapon laser, shockwave;
    public Collectible shotgun;
    public GameObject spike;
    public float spikeSpeed, spikeDistance, spikeDelay;
    const float GROUND = -2.5f;
    bool died = false;

    public override void OnSpawn () {
        shockCounter = 3;
        attackPattern = 0;
        attackCooldown = baseCooldown;
    }

    public override void OnDeath () {
        if (!died) {
            died = true;
            GameObject newItem = GameObject.Instantiate (shotgun, transform.position, Quaternion.identity).gameObject;
            newItem.GetComponent<Rigidbody2D> ().velocity = Vector2.up * 4f;
            base.OnDeath ();
        }
    }

    public override void Attack () {
        if (attackPattern == 1 && attack1 () ||
            attackPattern == 2 && attack2 () ||
            attackPattern == 3 && attack3 ()) {
            attackCooldown = baseCooldown + (health / 100f);
            attackPattern = 0;
        }
    }

    public override void MovePattern () {
        playerFound = player != null;
        if (playerFound) {
            if (attackCooldown > 0) {
                attackCooldown -= Time.deltaTime;
            } else {
                if (attackPattern == 0) {
                    if (shockCounter == 0) {
                        attackPattern = 3;
                        shockCounter = 3;
                    } else {
                        if (player.currState == Player.MoveState.Ground) {
                            attackPattern = 1;
                        } else {
                            attackPattern = 2;
                        }
                        shockCounter -= 1;
                    }
                    Attack ();
                }
            }
        }
    }

    // ground smash
    private bool attack1 () {
        Debug.Log ("ATTAK1!");
        shockwave.UseWeapon ();
        return true;
    }

    // laser
    private bool attack2 () {
        laser.UseWeapon ();
        return true;
    }

    // spike
    private bool attack3 () {
        StartCoroutine (MoveSpike (GameObject.Instantiate (spike, player.transform.position + (spikeDistance * Vector3.down), Quaternion.identity)));
        return true;
    }

    IEnumerator MoveSpike (GameObject spikeObj) {
        const float RESOLUTION = 0.05f;
        while (spikeObj.transform.position.y < GROUND) {
            spikeObj.transform.Translate (Vector3.up * RESOLUTION * spikeSpeed);
            yield return new WaitForSeconds (RESOLUTION);
        }
        // Destroy(spikeObj);
        yield return new WaitForSeconds (spikeDelay);

        while (spikeObj.transform.position.y > GROUND - spikeDistance) {
            spikeObj.transform.Translate (Vector3.down * RESOLUTION * spikeSpeed);
            yield return new WaitForSeconds (RESOLUTION);
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag.Equals("Player")) {
            player.TakeDamage(DMG);
        }
    }
}