using System.Collections;
using UnityEngine;

public class EnemyFracture : Enemy {

    private int shockCounter;

    // 0 = Not decided, 1 = Ground attk, 2 = Air attk, 3 = Shock attk.
    private int attackPattern;
    private float attackCooldown;
    public float COOLDOWN_MAX = 200;
    public Weapon laser;
    public Collectible shotgun;
    public GameObject spike;
    public float spikeSpeed, spikeDistance;

    public override void OnSpawn () {
        shockCounter = 3;
        attackPattern = 0;
        attackCooldown = COOLDOWN_MAX;
    }

    public override void OnDeath () {
        GameObject.Instantiate (shotgun, transform.position, Quaternion.identity);
        base.OnDeath ();
    }

    public override void Attack () {
        if (attackPattern == 1 && attack1 () ||
            attackPattern == 2 && attack2 () ||
            attackPattern == 3 && attack3 ()) {
            attackCooldown = COOLDOWN_MAX;
            attackPattern = 0;
        }
    }

    public override void MovePattern () {
        if (playerFound) {
            if (attackCooldown > 0) {
                attackCooldown -= 1;
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

        return true;
    }

    // laser
    private bool attack2 () {
        Debug.Log ("ATTAK2!");
        laser.UseWeapon ();
        return true;
    }

    // spike
    private bool attack3 () {
        StartCoroutine(MoveSpike(GameObject.Instantiate(spike, GameObject.FindObjectOfType<Player>().transform.position + (spikeDistance * Vector3.down), Quaternion.identity)));
        return true;
    }

    IEnumerator MoveSpike (GameObject spikeObj) {
        float moved = 0f;
        while (moved < spikeDistance) {
            print(moved);
            // spikeObj.transform.Translate(Vector3.up * Time.deltaTime * spikeDistance);
            moved += Time.deltaTime;
        }
        // Destroy(spikeObj);
        yield return null;
    }
}