using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    public RectTransform compassImage;   // Assign your wide compass bar image
    public float compassWidth = 0.7f;    // Width of your compass image
    public float maxHeading = 360f;       // Degrees in a full circle

    private float heading;                // Latest heading from server
    private float x, y;                   // Latest position (if needed)

    public TSSCommunicator TSS;                 // Your communication manager (TSS)

    private float fetchTimer = 0f;
    public float fetchInterval = 1.0f; // fetch every 1 second

    private async void Update()
    {
        fetchTimer += Time.deltaTime;
        if (fetchTimer >= fetchInterval)
        {
            fetchTimer = 0f;
            await UpdateFromServer();
        }

        UpdateCompass();
    }

    private async Task UpdateFromServer()
    {
        // Send command for X position
        await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 17, 0);
        if (TSS.HasNewData && TSS.LastCommandNumber == 17)
        {
            x = TSS.LastOutputData;
            TSS.setHasNewDataFalse();
        }

        // Send command for Y position
        await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 18, 0);
        if (TSS.HasNewData && TSS.LastCommandNumber == 18)
        {
            y = TSS.LastOutputData;
            TSS.setHasNewDataFalse();
        }

        // Send command for Heading
        await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 19, 0);
        if (TSS.HasNewData && TSS.LastCommandNumber == 19)
        {
            heading = TSS.LastOutputData;  // heading in degrees (0-360)
            TSS.setHasNewDataFalse();
            print("Received Heading: " + heading);
        }
    }

    private void UpdateCompass()
    {
        float normalizedHeading = heading / maxHeading;  // Normalize between 0 and 1
        float xOffset = normalizedHeading * compassWidth;

        print("Norm Heading: " + normalizedHeading);

        compassImage.anchoredPosition = new Vector2(-xOffset, compassImage.anchoredPosition.y);
    }
}


// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class CompassBar : MonoBehaviour
// {
//     public RectTransform compassBarTransform;
//     public RectTransform objectiveMarkerTransform;
//     public RectTransform northMarkerTransform;
//     public RectTransform southMarkerTransform;
//     public Transform cameraObjectTransform;
//     public Transform objectiveObjectTransform;

//     void Update()
//     {
//         SetMarkerPosition(objectiveMarkerTransform, objectiveObjectTransform.position);
//         SetMarkerPosition(northMarkerTransform, cameraObjectTransform.position + Vector3.forward * 1000);
//         SetMarkerPosition(southMarkerTransform, cameraObjectTransform.position + Vector3.back * 1000);
//     }

//     private void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPosition)
//     {
//         Vector3 directionToTarget = worldPosition - cameraObjectTransform.position;
//         float signedAngle = Vector3.SignedAngle(new Vector3(cameraObjectTransform.forward.x, 0, cameraObjectTransform.forward.z), new Vector3(directionToTarget.x, 0, directionToTarget.z), Vector3.up);

//         float compassPosition = Mathf.Clamp(signedAngle / Camera.main.fieldOfView, -0.5f, 0.5f);
//         markerTransform.anchoredPosition = new Vector2(compassBarTransform.rect.width * compassPosition, 0);
//     }
// }