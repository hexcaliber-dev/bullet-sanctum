using UnityEngine;
using System.Collections;

public class EnemyWeapon : Pistol {

    // Bullet projectile, cooldown, and cooldown time is inherited from Weapon.
    public bool canShoot = false; // Set to true when in range

    protected override void Start() {   
        base.Start();
        onCooldown = false;
        StartCoroutine(ShootLoop());
    }
    protected override void Update() {   
        base.Update();
        // TODO
    }

    public override void UseWeapon() {   
        // Do nothing. Shoot loop already established
    }

    IEnumerator ShootLoop() {    //cool down time
        while (true) {
            if (canShoot) {
                onCooldown = true;
                ShootAt(GameObject.FindObjectOfType<Player>().transform.position);
            }
            yield return new WaitForSeconds(cooldownTime);
            onCooldown = false;
        }
    }




}