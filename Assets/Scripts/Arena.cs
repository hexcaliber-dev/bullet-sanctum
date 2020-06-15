using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    public List<Transform> spawners;

    public Transform scrollSpawn;
    public GameObject scroll;
    public GameObject assignedDoors;

    public List<Enemy> enemies;
    public int currWave = 0;

    public bool debug; // If debug, disables automatic wave spawning
    public static bool cleared = false;

    public List<int[, ]> waves = new List<int[, ]> ();

    // Start is called before the first frame update
    void Start () {
        waves.Add (new int[, ] { { 5, 0 }, { 1, 1 }, { 3, 2 } });
        waves.Add (new int[, ] { { 2, 0 } });
        waves.Add (new int[, ] { { 0, 0 }, { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 }, { 5, 5 } });
        waves.Add (new int[, ] { { 1, 0 }, { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 } });
        waves.Add (new int[, ] { { 1, 0 }, { 3, 1 }, { 1, 2 }, { 0, 3 }, { 0, 4 }, { 3, 5 } });
        waves.Add (new int[, ] { { 1, 0 }, { 2, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 }, { 0, 1 }, { 0, 2 } });
        waves.Add (new int[, ] { { 4, 0 }, { 4, 1 }, { 4, 2 } });
        waves.Add (new int[, ] { { 3, 0 }, { 2, 1 }, { 4, 2 }, { 2, 3 }, { 3, 4 }, { 1, 5 }, { 1, 3 }, { 3, 4 } });
        waves.Add (new int[, ] { { 2, 0 }, { 2, 1 }, { 2, 2 }, { 2, 3 }, { 2, 4 }, { 1, 5 }, { 1, 4 }, { 1, 3 } });
        waves.Add (new int[, ] { { 5, 0 }, { 5, 0 }, { 5, 0 }, { 5, 0 }, { 5, 0 }, { 5, 0 }, { 5, 1 }, { 5, 1 }, { 5, 1 }, { 5, 1 }, { 5, 1 }, { 5, 1 }, { 5, 2 }, { 5, 2 }, { 5, 2 }, { 5, 2 }, { 5, 2 }, { 5, 2 } });
        waves.Add (new int[, ] { { 0, 0 }, { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 }, { 5, 5 }, { 0, 0 }, { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 }, { 5, 5 } });
        if (!debug && !cleared) {
            for (int i = 0; i < waves[currWave].GetLength (0); i += 1) {
                Spawn (waves[currWave][i, 0], waves[currWave][i, 1]);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (!debug) {
            int numEnemies = GameObject.FindObjectsOfType<Enemy> ().Length;
            if (numEnemies <= 2 && !cleared) {
                if (currWave < waves.Count - 1) {
                    currWave += 1;
                    for (int i = 0; i < waves[currWave].GetLength (0); i += 1) {
                        Spawn (waves[currWave][i, 0], waves[currWave][i, 1]);
                    }
                } else if (numEnemies == 0) {
                    if (!cleared) {
                        cleared = true;
                        GameObject.Instantiate (scroll, scrollSpawn.position, Quaternion.identity);
                        assignedDoors.SetActive(false);
                    }
                }
            }
        }
    }

    public void Spawn (int enemy, int spawner) {
        GameObject.Instantiate (enemies[enemy], spawners[spawner].position, Quaternion.identity);
    }

}