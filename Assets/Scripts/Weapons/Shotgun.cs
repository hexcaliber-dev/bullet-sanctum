using System.Collections;
using UnityEngine;

public class Shotgun : Weapon {

    public int numBullets; //specifically number of bullets per shotgun shot

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

        // Shoot numBullets number of bullets
        for (int i = 0; i <= numBullets; i++) {
            float randomNum = Random.Range (-45f, 45f) / 10f;
            print (randomNum);
            print (originalTarget.x * randomNum + " , " + originalTarget.y * randomNum);
            // for each, add a random amount to originalTarget to make it spread out
            base.ShootAt (new Vector2 (originalTarget.x * randomNum, originalTarget.y * randomNum));
        }

        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

}