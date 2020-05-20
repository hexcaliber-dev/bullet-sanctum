using System;
using UnityEngine;

public class Player : LivingEntity {

    const int STARTING_HEALTH = 20;

    public int maxHealth; // current health is inherited from LivingEntity

    public Weapon currWeapon;

    Rigidbody2D rb2D;

    public float accelStrength, decelStrength, jumpStrength, maxSpeed; // assign in inspector
    float currVelocity;

    void Start () {
        rb2D = GetComponent<Rigidbody2D> ();
        maxHealth = STARTING_HEALTH;
        health = STARTING_HEALTH;
    }

    void Update () {
        if (Input.GetKey (KeyCode.A)) {
            
        }
        if (Input.GetKey (KeyCode.D)) {

        }

        if (Input.GetKey (KeyCode.Mouse0)) {
            //shoot?
        }

        rb2D.velocity = new Vector2 (Mathf.Clamp (currVelocity, -maxSpeed, maxSpeed), 0);
    }

    public override void OnSpawn () {

    }

    public override void OnDeath () {

    }

    public override void Attack () {
        currWeapon.UseWeapon ();
    }

    public override void TakeDamage (Bullet b) {

    }
}