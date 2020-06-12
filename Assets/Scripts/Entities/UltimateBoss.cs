using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateBoss : Enemy {

    private bool cameraStop = false;
    private bool attack_state = false;
    private int attack_pattern = 1;
    private int attack_timr = 100;
    Animator animator;
    private List<Transform> wraith_spawns;

    // Enemies to spawn on attack #1 (Usually Wraiths).
    public GameObject spawnee;

    public override void OnSpawn() {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
    
        wraith_spawns = new List<Transform>();

        foreach (Transform child in transform) {
            wraith_spawns.Add(child);
        }
    }

    public override void Attack() {
        animator.SetBool("attacking", true);

        switch (attack_pattern) {
            case 1:
                attack1();
                break;
            default:
                break;
        }
    }

    private void attack1() {
        // Spawn Wraiths...
        attack_timr = 70;

        GameObject spawnee1 = Instantiate(spawnee, transform.position, Quaternion.identity) as GameObject;
        spawnee1.SendMessage("setTeleports", wraith_spawns);
    }

    public override void MovePattern() {
        cameraStop = Camera.main.GetComponent<CameraUtils>().hitBoundsRight;

        if (!attack_state && !cameraStop) {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        attack_timr -= 1;

        if (attack_timr < 0) {
            if (!attack_state) {
                Attack();

                attack_state = true;
            } else {
                animator.SetBool("attacking", false);

                attack_state = false;

                attack_timr = 200;
            }
        }
    }
}
