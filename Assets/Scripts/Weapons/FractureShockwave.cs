using System.Collections;
using UnityEngine;

public class FractureShockwave : Weapon {

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.
    public bool canShoot = false; // Set to true when in range

    protected override void Start () {
        base.Start ();
        onCooldown = false;
    }
    protected override void Update () {
        base.Update ();
        // TODO
    }

    public override void UseWeapon () {
        base.ShootAt ((Vector2) transform.position + Vector2.left);
        base.ShootAt ((Vector2) transform.position + Vector2.right);
    }
}