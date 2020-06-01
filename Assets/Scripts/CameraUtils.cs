using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour {
    private Camera cam;
    public float smoothSpd = 5f;
    public Vector3 offset = new Vector3 (0, 0, -10);
    public Transform bottomLeft, topRight; // Place empty gameObjects in the corners of the map

    Vector3 originalPos;
    GameObject player;

    // Start is called before the first frame update
    void Start () {
        // Get gameObject camera component.
        cam = GetComponent<Camera> ();
        player = GameObject.FindObjectOfType<Player>().gameObject;
        if (bottomLeft == null || topRight == null) {
            bottomLeft = GameObject.Find("BottomLeft").transform;
            topRight = GameObject.Find("TopRight").transform;
        }
    }

    // Camera follow script
    // TODO: Edge detection.
    void FixedUpdate () {
        // Get camera dimensions.
        float cameraWidth = cam.rect.width;
        float cameraHeight = cam.rect.height;

        // Destination vector.
        Vector3 desPos = player.GetComponent<Transform>().position + offset;
        // Add camera center offset.
        float worldCamHeight = cam.orthographicSize;
        float worldCamWidth = cam.orthographicSize * cam.aspect;

        desPos = new Vector3 (desPos.x + (cameraWidth / 2), desPos.y + (cameraHeight / 2), desPos.z);
        desPos = new Vector3 (Mathf.Clamp(desPos.x, bottomLeft.position.x + worldCamWidth, topRight.position.x - worldCamWidth),
                              Mathf.Clamp(desPos.y, bottomLeft.transform.position.y + worldCamHeight, topRight.transform.position.y - worldCamHeight),
                              desPos.z);
        // Lerp the camera for extra smoothness.
        transform.position = Vector3.Lerp (transform.position, desPos, smoothSpd * Time.deltaTime);
    }

    public void Shake (float duration, float amount) {
        StopAllCoroutines ();
        originalPos = transform.position;
        StartCoroutine (cShake (duration, amount));
    }

    IEnumerator cShake (float duration, float amount) {
        float endTime = Time.time + duration;

        while (Time.time < endTime) {
            transform.localPosition = originalPos + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    // Set a player to another object. Used for room switching.
    public void SetPlayer(GameObject newPlayer) {
        player = newPlayer;
    }
}