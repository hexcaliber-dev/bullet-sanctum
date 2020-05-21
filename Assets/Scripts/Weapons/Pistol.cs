using UnityEngine;
using System.Collections;

public class Pistol : Weapon {

    // Bullet projectile is inherited from Weapon.

    private bool onCooldown;
    public float cooldownTime; // in seconds

    protected override void Start() {
        base.Start();
        onCooldown = false;
    }
    protected override void Update() {
        base.Update();
        // TODO
    }

    public override void UseWeapon() {
        if (!onCooldown) {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot() {
        onCooldown = true;
        base.UseWeapon();
        yield return new WaitForSeconds(cooldownTime);
        onCooldown = false;
    }




}