using UnityEngine;

// Generic Bullet object that can be used for both players and enemy bullets.
public class Bullet : MonoBehaviour {
    public float speed;
    public float deleteTime;
    public int damage;
    protected Vector3 target;
    public int pierceCount, bounceCount;
    int currPierce, currBounce;

    public void SetTarget (Vector3 target) {
        this.target = target;
        // Point at mouse
        Vector3 diff = target - transform.position;
        diff.Normalize ();
        float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);
        currPierce = pierceCount;
        currBounce = bounceCount;
        GetComponent<Rigidbody2D> ().AddForce (Vector3.Normalize ((Vector2) (target - transform.position)) * speed * 10f);
    }

    public void SetTarget (GameObject target) {
        SetTarget (target.transform.position);
    }

    void Update () {
    }

    // REMEMBER TO SET COLLISION LAYERS FOR THE PREFAB! (so it doesn't collide with the sender)
    void OnCollisionEnter2D (Collision2D col) {
        // print ("COL");
        LivingEntity entity = col.gameObject.GetComponent<LivingEntity> ();
        if (entity != null) {
            entity.TakeDamage (this);
            if (pierceCount == 0)
                Destroy (gameObject);
            pierceCount -= 1;
        } else {
            if (bounceCount == 0)
                Destroy (gameObject);
            bounceCount -= 1;
        }
    }

    void Start () {
        // delete bullet after a certain amount of time
        Destroy (gameObject, deleteTime);
    }
}