using System.Collections;
using UnityEngine;

public class FractureLaser : Weapon {

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.
    public bool canShoot = false; // Set to true when in range
    public float laserGap;
    public int laserCount;

    protected override void Start () {
        base.Start ();
        onCooldown = false;
    }
    protected override void Update () {
        base.Update ();
        // TODO
    }

    public override void UseWeapon () {
        StartCoroutine (ShootLoop ());
    }

    IEnumerator ShootLoop () { //cool down time
        for (int i = 0; i < laserCount; i += 1) {
            Vector2 target = GameObject.FindObjectOfType<Player> ().transform.position;
            yield return new WaitForSeconds(laserGap);
            ShootAt(target);
        }
        yield return null;
    }

}