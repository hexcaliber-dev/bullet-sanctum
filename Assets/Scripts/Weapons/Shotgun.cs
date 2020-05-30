using System.Collections;
using UnityEngine;

public class Shotgun : Weapon {

    public int numBullets; //specifically number of bullets per shotgun shot
    public float knockback;

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.

    protected override void Start () {
        base.Start ();
        onCooldown = false;
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
        onCooldown = true;
        Vector3 originalTarget = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        GameObject.FindObjectOfType<CameraUtils> ().Shake (0.15f, 0.15f);

        // Shoot numBullets number of bullets
        for (int i = 0; i <= numBullets; i += 1) {
            float randomNum = Random.Range (-1.5f, 1.5f);
            // print (randomNum);
            // print (originalTarget.x + randomNum + " , " + originalTarget.y * randomNum);
            // for each, add a random amount to originalTarget to make it spread out
            base.ShootAt (new Vector2 (originalTarget.x + randomNum, originalTarget.y + randomNum));
        }

        // Knock back player
        GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D> ().velocity += (Vector2) (-(Vector3.Normalize ((Vector2) originalTarget - (Vector2) transform.position)) * knockback);

        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

}