using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassDirection : MonoBehaviour
{
    [Header("Compass Settings")]
    public Image compassImage; // Drag the compass UI Image here
    private RectTransform compassRect;

    [Header("Testing")]
    public bool testMode = false;
    public float testHeading = 0f;
    public float rotationSpeed = 30f; // Degrees per second

    private void Start()
    {
        if (compassImage != null)
        {
            compassRect = compassImage.rectTransform;
        }
        else
        {
            Debug.LogError("Compass Image not assigned!");
        }
    }

    private void Update()
    {
        if (compassRect == null) return;

        float heading = testMode ? GetTestHeading() : GetBackendHeading();

        if (heading < 0f) heading += 360f;

        // Rotate opposite to heading
        compassRect.localRotation = Quaternion.Euler(0f, 0f, -heading);
    }

    private float GetTestHeading()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            testHeading += rotationSpeed * Time.deltaTime;
            if (testHeading >= 360f) testHeading -= 360f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            testHeading -= rotationSpeed * Time.deltaTime;
            if (testHeading < 0f) testHeading += 360f;
        }

        return testHeading;
    }

    private float GetBackendHeading()
    {
        var backend = BackendDataService.Instance;
        if (backend == null || backend.LatestData == null ||
            backend.LatestData.eva1 == null || backend.LatestData.eva1.imu == null)
        {
            Debug.LogWarning("Waiting for backend heading data...");
            return 0f;
        }

        return backend.LatestData.eva1.imu.heading;
    }
}
