using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OblivionBob : MonoBehaviour
{
    public Transform upperLimit;
    public Transform lowerLimit;
    public float upSpeed;
    public float downSpeed;
    private bool goingUp;
    // Start is called before the first frame update
    void Start()
    {
        goingUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(goingUp)
        {
            if (gameObject.transform.position.y >= upperLimit.position.y)
            {
                goingUp = false;
            }
            else
            {
                gameObject.transform.Translate(Vector3.up * Time.deltaTime * upSpeed);
            }
        }
        else
        {
            if (gameObject.transform.position.y <= lowerLimit.position.y)
            {
                goingUp = true;
            }
            else
            {
                gameObject.transform.Translate(Vector3.down * Time.deltaTime * downSpeed);
            }
        }
    }
}
