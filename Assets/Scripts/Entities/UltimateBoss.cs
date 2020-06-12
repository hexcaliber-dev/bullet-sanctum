using System;
using UnityEngine;

public class UltimateBoss : Enemy {

    private bool attack_state = false;
    private int attack_timr = 100;
    Animator animator;

    public override void OnSpawn() {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
    }

    public override void Attack() {
        animator.SetBool("attacking", true);
    }

    public override void MovePattern() {
        if (!attack_state) {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        attack_timr -= 1;

        if (attack_timr < 0) {
            if (!attack_state) {
                Attack();

                attack_timr = 70;

                attack_state = true;
            } else {
                animator.SetBool("attacking", false);

                attack_state = false;

                attack_timr = 200;
            }
        }
    }
}