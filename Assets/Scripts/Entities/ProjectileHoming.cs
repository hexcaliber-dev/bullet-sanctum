using UnityEngine;

public class ProjectileHoming : Enemy {

	public Transform target;
	public float rotSpeed = 2000f;

	public override void OnSpawn() {
        rb = gameObject.GetComponent<Rigidbody2D> ();
		rb.gravityScale = 0;
	}

	public override void Attack() {}

	public void setTarget(Transform t) {
		target = t;
	}

	public override void MovePattern() {
		rb.velocity = transform.up * speed * Time.deltaTime;
		Vector3 targetVector = target.position - transform.position;
		float rotatingIndex = Vector3.Cross(targetVector, transform.up).z;
		rb.angularVelocity = -1 * rotatingIndex * rotSpeed * Time.deltaTime;
		speed += 0.1f;
	}

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
			player.TakeDamage(DMG);
			Destroy(gameObject);
		}
		if (collision.gameObject.tag.Equals ("Bullet")) {
			Bullet b = collision.gameObject.GetComponent<Bullet> ();
            TakeDamage(b);
			b.DoCollision(gameObject);
        }
	}
}