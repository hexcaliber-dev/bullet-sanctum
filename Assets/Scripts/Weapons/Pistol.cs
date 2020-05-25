using UnityEngine;
using System.Collections;

public class Pistol : Weapon {

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.

    protected override void Start() {   //just having no cooldown
        base.Start();
        onCooldown = false;
    }
    protected override void Update() {   //nothing here yet
        base.Update();
        // TODO
    }

    public override void UseWeapon() {   //shoot bullet
        if (!onCooldown) {
            StartCoroutine(Shoot());
        }
    }


    IEnumerator Shoot() {    //cool down time
        onCooldown = true;
        base.UseWeapon();
        yield return new WaitForSeconds(cooldownTime);
        onCooldown = false;
    }

}