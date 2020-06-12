using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can be shot by player. On destruction, drops specified loot.
public class WardenShield : LivingEntity {

    public float knockback, minBreakSpeed;
    int dmgState;
    public List<Sprite> damageSprites;
    public int damage;
    Player player;

    void OnCollisionEnter2D (Collision2D collision) {
        print("WARDEN COL");
        player = GameObject.FindObjectOfType<Player>();
        if (collision.gameObject.tag == "Player") {
            Vector2 kbVector = new Vector2 (knockback, 1f);
            if ((player.transform.position.x - transform.position.x) < 0) {
                kbVector.x = -kbVector.x;
            }
            player.GetComponent<Rigidbody2D>().AddForce (kbVector, ForceMode2D.Impulse);
            Attack ();
        }
        if (collision.gameObject.tag == "Bullet") {
            Bullet colBullet = collision.gameObject.GetComponent<Bullet> ();
            print(colBullet.speed);
            if (colBullet.speed >= minBreakSpeed) {
                dmgState += 1;
                if (dmgState >= damageSprites.Count) {
                    Destroy (gameObject);
                } else {
                    GetComponent<SpriteRenderer> ().sprite = damageSprites[dmgState];
                }
            }
        }
    }

    public override void OnSpawn () {
        dmgState = 0;
    }
    public override void Attack () {
        if (player != null) {
            player.TakeDamage(damage);
        }
    }
}