using UnityEngine;

public class EnemySpawnpoint : MonoBehaviour {

    // Assign this value in the inspector.
    public Enemy enemyToSpawn;

    void Start() {
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
    }

    void Update() {

    }
}