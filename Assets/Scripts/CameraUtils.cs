using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour {
    private Camera cam;
    public Transform player;
    public float smoothSpd = 5f;
    public Vector3 offset = new Vector3 (0, 0, -10);

    bool isShaking;

    // How long the object should shake for.
    public float shakeDuration = 0.25f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    Vector3 originalPos;

    // Start is called before the first frame update
    void Start () {
        // Get gameObject camera component.
        cam = GetComponent<Camera> ();
        isShaking = false;
    }

    // Camera follow script
    // TODO: Edge detection.
    void FixedUpdate () {
        // Get camera dimensions.
        float cameraWidth = cam.rect.width;
        float cameraHeight = cam.rect.height;

        // Destination vector.
        Vector3 desPos = player.position + offset;
        // Add camera center offset.
        desPos = new Vector3 (desPos.x + (cameraWidth / 2), desPos.y + (cameraHeight / 2), desPos.z);
        // Lerp the camera for extra smoothness.
        transform.position = Vector3.Lerp (transform.position, desPos, smoothSpd * Time.deltaTime);
    }

    public void Shake () {
        StopAllCoroutines ();
        originalPos = transform.position;
        StartCoroutine (cShake (shakeDuration, shakeAmount));
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

}