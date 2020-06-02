using System.Collections;
using UnityEngine;

public class Shotgun : Weapon {

    public int numBullets; //specifically number of bullets per shotgun shot
    public float knockback;
    public int numSecondaryShots;
    public Bullet pierceyBullets;
    public float secondaryDelay;
    public bool supercharged;
    public float superchargedReloadTime;
    const float spamBuffer = 0.25f; // stops player from spamming

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.

    protected override void Start () {
        base.Start ();
        StopAllCoroutines ();
        onCooldown = false;
        onSecondaryCooldown = Shop.currShotUpgrade < 1;
        StartCoroutine (SuperchargeTimer ());
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
        Vector3 originalTarget = Camera.main.ScreenToWorldPoint (Input.mousePosition);

        if (Shop.currShotUpgrade > 2) {
            projectile = pierceyBullets;
        }

        Fire (originalTarget);
        GameObject.FindObjectOfType<CameraUtils> ().Shake (0.15f, 0.15f);
        // Knock back player
        GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D> ().velocity += (Vector2) (-(Vector3.Normalize ((Vector2) originalTarget - (Vector2) transform.position)) * knockback);
        // Repeater upgrade
        onCooldown = true;
        if (Shop.currShotUpgrade > 1 && supercharged) {
            supercharged = false;
            StartCoroutine (SuperchargeTimer ());
            GameObject.FindObjectOfType<HUD> ().ResetPrimaryMeter();
            yield return new WaitForSeconds (0.25f);
            onCooldown = false;
        } else {
            yield return new WaitForSeconds (cooldownTime);
            onCooldown = false;
            StartCoroutine (SuperchargeTimer ());
        }

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
            GameObject.FindObjectOfType<Player> ().ResetVelocity ();
            ShootAt (currentTarget);
            currentTarget = (Vector2) transform.position + new Vector2 (Mathf.Cos (2 * Mathf.PI / numSecondaryShots * i), Mathf.Sin (2 * Mathf.PI / numSecondaryShots * i));
            if (i % 5 == 4)
                yield return new WaitForSeconds (secondaryDelay);
        }
        yield return new WaitForSeconds (secondaryCooldownTime - (secondaryDelay * numSecondaryShots));
        onSecondaryCooldown = false;
    }

    public IEnumerator SuperchargeTimer () {
        yield return new WaitForSeconds (superchargedReloadTime);
        if (Shop.currShotUpgrade > 1 && !onCooldown) {
            supercharged = true;
            GameObject.FindObjectOfType<HUD> ().OverchargeMeter ();
        }
    }
}