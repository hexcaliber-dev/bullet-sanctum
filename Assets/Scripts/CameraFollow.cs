using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Camera cam;
    public Transform player;
    public float smoothSpd = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    // Start is called before the first frame update
    void Start()
    {
        // Get gameObject camera component.
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    // TODO: Edge detection.
    void FixedUpdate()
    {
        // Get camera dimensions.
        float cameraWidth = cam.rect.width;
        float cameraHeight = cam.rect.height;

        // Destination vector.
        Vector3 desPos = player.position + offset;
        // Add camera center offset.
        desPos = new Vector3(desPos.x + (cameraWidth/2), desPos.y + (cameraHeight/2), desPos.z);
        // Lerp the camera for extra smoothness.
        transform.position = Vector3.Lerp(transform.position, desPos, smoothSpd * Time.deltaTime);
    }
}
