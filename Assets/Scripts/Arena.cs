using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    public List<Transform> spawners;
    public List<Enemy> enemies;
    public static int currWave = 0;

    public List<int[, ]> waves = new List<int[, ]> ();

    // Start is called before the first frame update
    void Start () {
        waves.Add (new int[, ] { { 4, 0 }, { 1, 1 }, { 3, 2 } });
        waves.Add (new int[, ] { { 0, 0 }, { 0, 1 }, { 3, 2 }, { 0, 3 }, { 0, 4 }, { 0, 5 } });
        waves.Add (new int[, ] { { 1, 0 }, { 1, 1 }, { 1, 2 } });
        waves.Add (new int[, ] { { 1, 0 }, { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 } });
        waves.Add (new int[, ] { { 1, 0 }, { 1, 1 }, { 1, 2 }, { 0, 3 }, { 0, 4 }, { 0, 5 } });
        waves.Add (new int[, ] { { 1, 0 }, { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 }, { 0, 6 }, { 0, 7 } });
        waves.Add (new int[, ] { { 2, 0 }, { 2, 1 }, { 2, 2 } });
        waves.Add (new int[, ] { { 2, 0 }, { 2, 1 }, { 2, 2 }, { 2, 3 }, { 2, 4 }, { 1, 5 }, { 1, 6 }, { 1, 7 } });
        waves.Add (new int[, ] { { 2, 0 }, { 2, 1 }, { 2, 2 }, { 2, 3 }, { 2, 4 }, { 1, 5 }, { 1, 6 }, { 1, 7 }, { 0, 8 }, { 0, 9 } });
        for (int i = 0; i < waves[currWave].GetLength (0); i += 1) {
            Spawn (waves[currWave][i, 0], waves[currWave][i, 1]);
        }
    }

    // Update is called once per frame
    void Update () {
        int numEnemies = GameObject.FindObjectsOfType<Enemy> ().Length;
        if (numEnemies <= 2) {
            for (int i = 0; i < waves[currWave].GetLength (0); i += 1) {
                Spawn (waves[currWave][i, 0], waves[currWave][i, 1]);
            }

            if (currWave < waves.Count - 1)
                currWave += 1;
        }
    }

    void Spawn (int enemy, int spawner) {
        GameObject.Instantiate (enemies[enemy], spawners[spawner].position, Quaternion.identity);
    }

}