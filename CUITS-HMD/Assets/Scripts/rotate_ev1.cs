using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_ev1 : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform spriteTransform;
    private float currentZRotation = 0f;
    
    [Header("Testing")]
    public bool testMode = false;
    public float testHeading = 0f;
    public float rotationSpeed = 30f; // Degrees per second

    void Start()
    {
        // Get components from this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteTransform = transform;
        
        if (spriteRenderer == null)
        {
            Debug.LogError("This script must be attached to a GameObject with a SpriteRenderer component!");
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        if (testMode)
        {
            // Allow manual control with keyboard
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                testHeading += rotationSpeed * Time.deltaTime;
                if (testHeading >= 360f)
                    testHeading -= 360f;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                testHeading -= rotationSpeed * Time.deltaTime;
                if (testHeading < 0f)
                    testHeading += 360f;
            }

            // Rotate the sprite based on test heading
            float rotationZ = -testHeading;
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            currentZRotation = rotationZ;
        }
        else
        {
            var backend = BackendDataService.Instance;

            if (backend == null || backend.LatestData == null || backend.LatestData.eva1 == null || backend.LatestData.eva1.imu == null)
            {
                Debug.LogWarning("Waiting for backend heading data...");
                return;
            }

            float heading = backend.LatestData.eva1.imu.heading;

            if (heading < 0f)
                heading += 360f;

            // Rotate the sprite in opposite direction so it looks like you're turning
            float rotationZ = -heading;
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            currentZRotation = rotationZ;
        }
    }
}
