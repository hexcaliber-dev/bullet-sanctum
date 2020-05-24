using UnityEngine;

public class Weapon : MonoBehaviour {

    public Bullet projectile;

    public bool onCooldown;
    public float cooldownTime; // in seconds

    // Default weapon behavior: spawn a bullet
    public virtual void UseWeapon () {
        Bullet b = GameObject.Instantiate (projectile, transform.position, Quaternion.identity);
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        b.SetTarget (new Vector3 (mousePoint.x, mousePoint.y, 0));
    }

    protected virtual void Start () { }

    protected virtual void Update () {
    }
}