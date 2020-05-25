using System.Collections;
using UnityEngine;

public class Shotgun : Weapon {

    public int numBullets;

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
        // for (i = 0; int <= numBullets; i++) {
        //  // for each, add a random amount to originalTarget to make it spread out
        //     Vector2, base.ShootAt(originalTarget + something)
        // }
        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

}