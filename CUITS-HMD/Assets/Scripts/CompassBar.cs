using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{

    public Vector3 waypointPos;
    public Transform viewDirection;


    public RectTransform compassImage;   // Assign your wide compass bar image
    public RectTransform waypointMarker; // Assign your waypoint marker image

    public float compassWidth;    // Width of your compass image
    public float maxHeading = 360f;       // Degrees in a full circle

    private float heading;                // Latest heading from server
    private float x, y;                   // Latest position (if needed)

    public TSSCommunicator TSS;                 // Your communication manager (TSS)

    //private float fetchTimer = 0f;
    //public float fetchInterval = 1.0f; // fetch every 1 second
    private float headingFetchTimer = 0f;
    private float positionFetchTimer = 0f;
    public float headingFetchInterval = 1.0f;
    public float positionFetchInterval = 2.0f;  // Position can update less frequently

    private float correctionOffsetAngle = 0f;

    private void Start(){
        x=0;
        y=0;
        heading = 0;
    }

    private async void LateUpdate()
    {
        headingFetchTimer += Time.deltaTime;
        positionFetchTimer += Time.deltaTime;

        if (headingFetchTimer >= headingFetchInterval)
        {
            headingFetchTimer = 0f;
            await UpdateHeading();
        }

        if (positionFetchTimer >= positionFetchInterval)
        {
            positionFetchTimer = 0f;
            await UpdatePosition();
        }

        UpdateCompass();
    }

    private async Task UpdateHeading()
    {
        try
        {
            //print("Fetching Heading");
            await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 19, 0);
            if (TSS.HasNewData && TSS.LastCommandNumber == 19)
            {
                heading = TSS.LastOutputData;
                TSS.setHasNewDataFalse();
                //print("Received Heading: " + heading);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating heading: " + e.Message);
        }
    }

    private async Task UpdatePosition()
    {
        try
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
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating position: " + e.Message);
        }
    }

    private void UpdateCompass()
    {
        //Vector3 forwardVector = Vector3.ProjectOnPlane(viewDirection.forward, Vector3.up).normalized;
        //float forwardSignedAngle = Vector3.SignedAngle(forwardVector, Vector3.forward, Vector3.up);

        //scroll compass bar using only TSS data
        float compassOffset = ((360-heading) / 360f) * compassWidth; //using 360-heading to flip direction; 90 degrees from north is east instead of west
        float wrappedOffset = Mathf.Repeat(compassOffset + (compassWidth / 2f), compassWidth) - (compassWidth / 2f);
        compassImage.anchoredPosition = new Vector3(wrappedOffset, 0);


        //calculate angle to waypoint
        Vector3 myPos = new Vector3(x, y, 0);
        Vector3 directionToWaypoint = waypointPos - myPos;
        float waypointAngle = Vector3.SignedAngle(directionToWaypoint, Vector3.up, Vector3.forward);

        // Convert to 0-360 range
        if (waypointAngle < 0)
        {
            waypointAngle += 360f;
        }

        // adjust waypoint marker
        float relativeWaypointAngle = (heading- waypointAngle) % 360;
        float waypointOffset = ((360-relativeWaypointAngle) / 360f) * compassWidth;
        float wrappedWaypointOffset = Mathf.Repeat(waypointOffset + (compassWidth / 2f), compassWidth) - (compassWidth / 2f);
        waypointMarker.anchoredPosition = new Vector3(wrappedWaypointOffset, 0);
    }
}