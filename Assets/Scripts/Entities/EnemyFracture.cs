using UnityEngine;
using System;

public class EnemyFracture : Enemy {

    private int shockCounter;
    
    // 0 = Not decided, 1 = Ground attk, 2 = Air attk, 3 = Shock attk.
    private int attackPattern;
    private float attackCooldown;
    public float COOLDOWN_MAX = 200;

    public override void OnSpawn() {
        shockCounter = 3;
        attackPattern = 0;
        attackCooldown = COOLDOWN_MAX;
    }

    public override void OnDeath() {}

    public override void Attack() {
        if (attackPattern == 1 && attack1() ||
            attackPattern == 2 && attack2() ||
            attackPattern == 3 && attack3()) {
            attackCooldown = COOLDOWN_MAX;
            attackPattern = 0;
        }
    }

    public override void MovePattern() {
        if (playerFound) {
            if (attackCooldown > 0) {
                attackCooldown -= 1;
            } else {
                if (attackPattern == 0) {
                    if (shockCounter == 0) {
                        attackPattern = 3;
                        shockCounter = 3;
                    } else {
                        if (player.currState == Player.MoveState.Ground) {
                            attackPattern = 1;
                        } else {
                            attackPattern = 2;
                        }
                        shockCounter -= 1;
                    }
                    Attack();
                }
            }
        }
    }

    private Boolean attack1() {
        Debug.Log("ATTAK1!");
        return true;
    }
    private Boolean attack2() {
        Debug.Log("ATTAK2!");
        return true;
    }
    private Boolean attack3() {
        Debug.Log("ATTAK3!");
        return true;
    }
}
