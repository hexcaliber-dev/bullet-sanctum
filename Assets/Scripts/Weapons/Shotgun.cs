using System.Collections;
using UnityEngine;

public class Shotgun : Weapon {

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
        for (int i = 0; i < 3; i += 1) {
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            ShootAt(mousePoint);
        }
    }

    IEnumerator Shoot () {
        onCooldown = true;
        base.UseWeapon ();
        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

}