using System.Collections;
using UnityEngine;

public class Pistol : Weapon {

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.
    public int numSecondaryShots;
    public float secondaryDelay;
    public Bullet superchargedBullet, bouncyBullet;
    public bool supercharged;
    public float superchargedReloadTime;

    float superchargedTimer;

    protected override void Start () {
        base.Start ();
        onCooldown = false;
        onSecondaryCooldown = Shop.currPistolUpgrade < 1;

        StartCoroutine (SuperchargeTimerTick ());
    }
    protected override void Update () {
        base.Update ();
        // TODO
    }

    public override void UseWeapon () {
        if (!onCooldown) {
            if (Shop.currPistolUpgrade > 2)
                projectile = bouncyBullet;
            if (supercharged) {
                StartCoroutine (SuperchargedShoot ());
            } else {
                StartCoroutine (Shoot ());
            }
        }
    }

    public override void UseSecondary () {
        if (!onSecondaryCooldown)
            StartCoroutine (SecondaryFire ());
    }

    IEnumerator Shoot () { //cool down time
        onCooldown = true;
        superchargedTimer = 0f;
        base.UseWeapon ();
        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

    IEnumerator SecondaryFire () {
        onSecondaryCooldown = true;
        Vector2 mousePoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        for (int i = 0; i < numSecondaryShots; i += 1) {
            print (mousePoint);
            print (-mousePoint);
            ShootAt (mousePoint);
            ShootAt ((Vector2) transform.position + ((Vector2) transform.position - mousePoint));
            yield return new WaitForSeconds (secondaryDelay);
        }
        yield return new WaitForSeconds (secondaryCooldownTime);
        onSecondaryCooldown = false;
    }

    IEnumerator SuperchargedShoot () {
        onCooldown = true;
        supercharged = false;
        superchargedTimer = 0f;
        GameObject.FindObjectOfType<CameraUtils> ().Shake (0.25f, 0.1f);
        Vector2 target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Bullet b = GameObject.Instantiate (superchargedBullet, transform.position, Quaternion.identity);
        b.SetTarget (new Vector3 (target.x, target.y, 0));
        yield return new WaitForSeconds (cooldownTime);
        onCooldown = false;
    }

    IEnumerator SuperchargeTimerTick () {
        const float RESOLUTION = 0.25f; // update time
        while (true) {
            superchargedTimer += RESOLUTION;
            if (superchargedTimer >= superchargedReloadTime && Shop.currPistolUpgrade > 1) {
                supercharged = true;
                GameObject.FindObjectOfType<HUD> ().OverchargeMeter ();
            }
            yield return new WaitForSeconds (RESOLUTION);
        }
    }
}