using System.Collections;
using UnityEngine;

public class Shotgun : Weapon {

    public int numBullets; //specifically number of bullets per shotgun shot
    public float knockback;
    public int numSecondaryShots;
    public float repeaterDelay;
    public Bullet pierceyBullets;
    public float secondaryDelay;

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.

    protected override void Start () {
        base.Start ();
        onCooldown = false;
        onSecondaryCooldown = Shop.currShotUpgrade < 1;
    }
    protected override void Update () {
        base.Update ();
        // TODO
    }

    public override void UseWeapon () {
        if (!onCooldown) {
            StartCoroutine (Shoot ());
        }
    }

    IEnumerator Shoot () {

        if (Shop.currShotUpgrade > 2) {
            projectile = pierceyBullets;
        }

        onCooldown = true;
        Vector3 originalTarget = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Fire (originalTarget);
        GameObject.FindObjectOfType<CameraUtils> ().Shake (0.15f, 0.15f);

        // Knock back player
        GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D> ().velocity += (Vector2) (-(Vector3.Normalize ((Vector2) originalTarget - (Vector2) transform.position)) * knockback);

        // Repeater upgrade
        if (Shop.currShotUpgrade > 1) {
            yield return new WaitForSeconds (repeaterDelay);
            Fire (originalTarget);
        }

        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

    void Fire (Vector3 originalTarget) {
        // Shoot numBullets number of bullets
        for (int i = 0; i <= numBullets; i += 1) {
            float randomNum = Random.Range (-1.5f, 1.5f);
            // for each, add a random amount to originalTarget to make it spread out
            base.ShootAt (new Vector2 (originalTarget.x + randomNum, originalTarget.y + randomNum));
        }
    }

    public override void UseSecondary () {
        if (!onSecondaryCooldown)
            StartCoroutine (SecondaryFire ());
    }

    IEnumerator SecondaryFire () {
        onSecondaryCooldown = true;
        Vector2 currentTarget = Vector2.right;
        GameObject.FindObjectOfType<CameraUtils> ().Shake (0.15f, 0.15f);
        for (int i = 0; i < numSecondaryShots; i += 1) {
            GameObject.FindObjectOfType<Player>().ResetVelocity();
            ShootAt (currentTarget);
            currentTarget = (Vector2)transform.position + new Vector2 (Mathf.Cos (2 * Mathf.PI / numSecondaryShots * i), Mathf.Sin (2 * Mathf.PI / numSecondaryShots * i));
            if (i % 5 == 4)
                yield return new WaitForSeconds(secondaryDelay);
        }
        yield return new WaitForSeconds (secondaryCooldownTime - (secondaryDelay * numSecondaryShots));
        onSecondaryCooldown = false;
    }
}