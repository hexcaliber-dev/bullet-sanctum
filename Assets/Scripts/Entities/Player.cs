using UnityEngine;
using System;

public class Player : LivingEntity {

    const int STARTING_HEALTH = 20;

    public int maxHealth; // current health is inherited from LivingEntity

    public Weapon currWeapon;

    void Start() {
        maxHealth = STARTING_HEALTH;
        health = STARTING_HEALTH;
    }

    void Update() {
        if (Input.GetKey(KeyCode.A)) {
            // run method here
        }

        if (Input.GetKey(KeyCode.Mouse0));
    }

    public override void OnSpawn() {

    }

    public override void OnDeath() {

    }

    public override void Attack() {
        currWeapon.UseWeapon();
    }

    public override void TakeDamage(Bullet b) {

    }
}