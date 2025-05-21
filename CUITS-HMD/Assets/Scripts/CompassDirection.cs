using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassDirection : MonoBehaviour
{
    public Image compassImage; // Drag your compass Image here in inspector
    private RectTransform compassRect;
    private float currentZRotation = 0f; 
    
    [Header("Testing")]
    public bool testMode = false;
    public float testHeading = 0f;
    public float rotationSpeed = 30f; // Degrees per second

    void Start()
    {
        // Get RectTransform from the compass image
        if (compassImage != null)
        {
            compassRect = compassImage.rectTransform;
        }
        else
        {
            Debug.LogError("Compass Image not assigned!");
        }
    }

    void Update()
    {
        if (compassRect == null) return;

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

            // Rotate the compass image based on test heading
            float rotationZ = -testHeading;
            compassRect.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
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

            // Rotate the compass image in opposite direction so it looks like you're turning
            float rotationZ = -heading;
            compassRect.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            currentZRotation = rotationZ;
        }
    }
}












