using System.Collections;
using UnityEngine;

// Explosive enemy.
public class EnemyShell : EnemyGhost {

    public int explodeDamage, explodeRange;
    public GameObject explosionAnimation;
    bool exploding = false;

    public override void OnSpawn () {
        base.OnSpawn();
        GetComponent<SpriteRenderer> ().flipX = false;
    }

    public override void OnDeath () {
        StartCoroutine (Explode ());
    }

    IEnumerator Explode () {
        if (!exploding) {
            exploding = true;
            GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
            GetComponent<Animator> ().SetBool ("exploding", true);
            yield return new WaitForSeconds (1f);
            GameObject.Instantiate (explosionAnimation, transform.position, Quaternion.identity);
            foreach (LivingEntity entity in GameObject.FindObjectsOfType<LivingEntity> ()) {
                if (!entity.entityName.Equals ("Shell") && Vector2.Distance (transform.position, entity.transform.position) < explodeRange) {
                    print ("DAMAGE" + entity.entityName);
                    entity.TakeDamage (explodeDamage);
                }
            }
            base.OnDeath ();
        }
    }

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag.Equals ("Player")) {
            player.TakeDamage(DMG);
            StartCoroutine (Explode ());
        }
    }
}