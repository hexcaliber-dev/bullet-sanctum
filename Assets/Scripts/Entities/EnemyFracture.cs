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
    public GameObject spike, infoPillar;
    public float spikeSpeed, spikeDistance, spikeDelay;
    public bool hasDoors;
    public GameObject assignedDoors;
    const float GROUND = -2.5f;
    static bool died = false;

    public override void OnSpawn () {
        if (died) {
            OnDeath ();
        }
        shockCounter = 3;
        attackPattern = 0;
        attackCooldown = baseCooldown;
    }

    public override void OnDeath () {
        if (hasDoors) {
            assignedDoors.SetActive (false);
        }
        if (!died) {
            died = true;
            GameObject newItem = GameObject.Instantiate (shotgun, transform.position, Quaternion.identity).gameObject;
            newItem.GetComponent<Rigidbody2D> ().velocity = Vector2.up * 4f;
        } else {
            bounty = 0;
        }
        infoPillar.SetActive (true);
        base.OnDeath ();
    }

    public override void Attack () {
        StartCoroutine (cAttack ());
    }

    IEnumerator cAttack () {
        GetComponent<Animator> ().SetBool ("attacking", true);
        AudioHelper.PlaySound ("fracture_buildup");
        yield return new WaitForSeconds (1f);
        if (attackPattern == 1 && attack1 () ||
            attackPattern == 2 && attack2 () ||
            attackPattern == 3 && attack3 ()) {
            attackCooldown = baseCooldown + (health / 100f);
            attackPattern = 0;
        }
        GetComponent<Animator> ().SetBool ("attacking", false);
    }

    public override void MovePattern () {
        playerFound = player != null;
        if (playerFound) {
            GetComponent<SpriteRenderer> ().flipX = (transform.position - player.transform.position).x < 0;
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
        base.MovePattern ();
    }

    // ground smash
    private bool attack1 () {
        AudioHelper.PlaySound ("fracture_rumbling");
        StartCoroutine (MoveSpike (GameObject.Instantiate (spike, player.transform.position + (spikeDistance * Vector3.down), Quaternion.identity)));
        return true;
    }

    // laser
    private bool attack2 () {
        laser.UseWeapon ();
        return true;
    }

    // spike
    private bool attack3 () {
        AudioHelper.PlaySound ("fracture_shockwave");
        shockwave.UseWeapon ();
        return true;
    }

    IEnumerator MoveSpike (GameObject spikeObj) {
        const float RESOLUTION = 0.05f;
        while (spikeObj != null && spikeObj.transform.position.y < GROUND) {
            spikeObj.transform.Translate (Vector3.up * RESOLUTION * spikeSpeed);
            yield return new WaitForSeconds (RESOLUTION);
        }
        // Destroy(spikeObj);
        yield return new WaitForSeconds (spikeDelay);

        while (spikeObj != null && spikeObj.transform.position.y > GROUND - spikeDistance) {
            spikeObj.transform.Translate (Vector3.down * RESOLUTION * spikeSpeed);
            yield return new WaitForSeconds (RESOLUTION);
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag.Equals ("Player")) {
            player.TakeDamage (DMG);
        }
    }
}