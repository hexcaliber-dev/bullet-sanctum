using System.Collections;
using UnityEngine;

public class EnemyWeapon : Weapon {

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.
    public bool canShoot = false; // Set to true when in range
    public bool manualControl;

    protected override void Start () {
        base.Start ();
        onCooldown = false;
        StartCoroutine (ShootLoop ());
    }
    protected override void Update () {
        base.Update ();
        // TODO
    }

    public override void UseWeapon () {
        if (manualControl)
            ShootAt (GameObject.FindObjectOfType<Player> ().transform.position);
    }

    IEnumerator ShootLoop () { //cool down time
        while (!manualControl) {
            if (canShoot) {
                onCooldown = true;
                ShootAt (GameObject.FindObjectOfType<Player> ().transform.position);
                if (GetComponent<SpriteRenderer> ().enabled)
                    AudioHelper.PlaySound ("ranger_shoot");
            }
            yield return new WaitForSeconds (cooldownTime);
            onCooldown = false;
        }
    }

}