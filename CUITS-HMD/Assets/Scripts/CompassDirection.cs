using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassDirection : MonoBehaviour
{
    public RectTransform compassImage; // ← Drag your Image (compass) here in inspector
    private float currentZRotation = 0f;

    void Update()
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
        compassImage.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        currentZRotation = rotationZ;
    }
}

