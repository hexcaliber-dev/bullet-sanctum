using UnityEngine;

public class FractureShockBullet : Bullet {
    public float growRate;


    void Update () { 
        transform.localScale += new Vector3(1, 1, 1) * growRate * Time.deltaTime;
    }
}