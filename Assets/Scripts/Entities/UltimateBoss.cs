using System;
using UnityEngine;

public class UltimateBoss : Enemy {

    public override void OnSpawn() {
        rb = gameObject.GetComponent<Rigidbody2D> ();
    }

    public override void Attack() {

    }

    public override void MovePattern() {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}