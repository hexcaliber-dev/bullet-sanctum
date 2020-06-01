using UnityEngine;

public class Weapon : MonoBehaviour {

    public Bullet projectile;

    public bool onCooldown, onSecondaryCooldown;
    public float cooldownTime; // in seconds
    public float secondaryCooldownTime;

    // Default weapon behavior: spawn a bullet. Level is the upgrade level of this weapon.
    public virtual void UseWeapon () {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        ShootAt(mousePoint);
    }

    public virtual void UseSecondary() {}

    protected virtual void Start () { }

    protected virtual void Update () { }

    protected void ShootAt (Vector2 target) {
        Bullet b = GameObject.Instantiate (projectile, transform.position, Quaternion.identity);
        b.SetTarget (new Vector3 (target.x, target.y, 0));
    }
}

