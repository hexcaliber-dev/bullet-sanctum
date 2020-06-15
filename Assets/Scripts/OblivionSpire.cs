using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OblivionSpire : MonoBehaviour
{
    public float transcriptionSpeed;
    
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.up * Time.deltaTime * transcriptionSpeed);
    }
}
