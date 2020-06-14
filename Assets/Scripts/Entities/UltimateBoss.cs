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
    public GameObject spawnee1;
    public GameObject spawnee2;

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
            case 2:
                attack2();
                break;
            default:
                break;
        }

        attack_pattern += 1;

        if (attack_pattern > 2) {
            attack_pattern = 1;
        }
    }

    private void attack1() {
        // Spawn Wraiths...
        attack_timr = 70;

        GameObject spawneeObj = Instantiate(spawnee1, transform.position, Quaternion.identity) as GameObject;
        spawneeObj.SendMessage("setTeleports", wraith_spawns);
    }

    private void attack2() {
        // Spawn homing
        attack_timr = 70;
        Vector3 spawnPoint = transform.position + (Vector3.up * 3);
        spawnPoint.z = -1;

        GameObject spawneeObj = Instantiate(spawnee2, spawnPoint, Quaternion.Euler(0, 0, -90)) as GameObject;
        spawneeObj.SendMessage("setTarget", player.transform);
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
