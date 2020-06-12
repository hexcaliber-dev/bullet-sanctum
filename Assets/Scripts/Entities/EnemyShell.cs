using System.Collections;
using UnityEngine;

// Explosive enemy.
public class EnemyShell : EnemyGhost {

    public int explodeDamage, explodeRange;
    public GameObject explosionAnimation;
    public bool toScaleExplosion;

    public float explodeDelay;
    bool exploding = false;

    public override void OnSpawn () {
        base.OnSpawn ();
        GetComponent<SpriteRenderer> ().flipX = false;
        if (toScaleExplosion)
            explodeRange *= (int) (transform.localScale.x / 3);
    }

    public override void OnDeath () {
        StartCoroutine (Explode ());
    }

    IEnumerator Explode () {
        if (!exploding) {
            exploding = true;
            AudioHelper.PlaySound ("shell_explode");
            GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
            GetComponent<Animator> ().SetBool ("exploding", true);
            yield return new WaitForSeconds (explodeDelay);
            GameObject explosion = GameObject.Instantiate (explosionAnimation, transform.position, Quaternion.identity);
            if (toScaleExplosion)
                explosion.transform.localScale = new Vector3 (transform.localScale.x * 3, transform.localScale.y * 3, transform.localScale.z * 3);
            foreach (LivingEntity entity in GameObject.FindObjectsOfType<LivingEntity> ()) {
                if (!entity.entityName.Equals ("Shell") && Vector2.Distance (transform.position, entity.transform.position) < explodeRange) {
                    print ("DAMAGE" + entity.entityName);
                    entity.TakeDamage (explodeDamage);
                }
            }
            GameObject.FindObjectOfType<CameraUtils> ().Shake (0.25f, 0.25f);
            base.OnDeath ();
        }
    }

    void OnCollisionEnter2D (Collision2D col) {
        if (col.gameObject.tag.Equals ("Player")) {
            // player.TakeDamage (DMG);
            StartCoroutine (Explode ());
        }
    }

    public override void TakeDamage (Bullet b) {
        base.TakeDamage (b);
        AudioHelper.PlaySound ("shell_hit", 0.3f);
    }
}