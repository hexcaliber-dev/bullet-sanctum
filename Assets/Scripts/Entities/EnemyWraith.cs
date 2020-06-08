using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A teleporting enemy that fires bolts at the player.
public class EnemyWraith : Enemy {

    public float teleportFrequency, preShootDelay, midShootDelay, fadeFrequency;
    SpriteRenderer sprite;
    public List<Transform> teleportPositions;
    int currPosition;

    public override void OnSpawn () {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        sprite = GetComponent<SpriteRenderer> ();
        StartCoroutine (BehaviorLoop ());
    }

    public override void Attack () {
        GetComponentInChildren<EnemyWeapon> ().UseWeapon ();
    }

    // This is inherited from LivingEntity
    // public override void OnDeath() {
    //     // Fucking die.
    // }

    public override void MovePattern () {
        sprite.flipX = (transform.position - player.transform.position).x < 0;
    }

    IEnumerator BehaviorLoop () {
        while (true) {
            int newPosition = currPosition;
            while (newPosition == currPosition) {
                newPosition = Random.Range (0, teleportPositions.Count);
            }
            transform.position = teleportPositions[newPosition].position;
            currPosition = newPosition;

            yield return new WaitForSeconds (teleportFrequency);
            while (sprite.color.a < 1f) {
                sprite.color = new Color (1, 1, 1, Mathf.Min (1, sprite.color.a + (fadeFrequency * Time.deltaTime)));
                yield return new WaitForSeconds (Time.deltaTime);
            }
            GetComponent<BoxCollider2D> ().enabled = true;
            rb.simulated = true;
            yield return new WaitForSeconds (preShootDelay);
            GetComponent<Animator> ().SetBool ("attacking", true);
            Attack ();
            yield return new WaitForSeconds (midShootDelay);
            Attack ();
            GetComponent<Animator> ().SetBool ("attacking", false);
            yield return new WaitForSeconds (preShootDelay);
            GetComponent<BoxCollider2D> ().enabled = false;
            rb.simulated = false;
            while (sprite.color.a > 0f) {
                sprite.color = new Color (1, 1, 1, Mathf.Max (0, sprite.color.a - (fadeFrequency * Time.deltaTime)));
                yield return new WaitForSeconds (Time.deltaTime);
            }
        }
    }
}