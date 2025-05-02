using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading.Tasks;

public class displayWaypoints : MonoBehaviour
{
    // Target to point towards
    public Vector3 waypointPos;
    public Vector3 myPos;

    private TSSCommunicator TSS;

    private float x;
    private float y;

    

    void Start(){
        TSS = FindObjectOfType(typeof(TSSCommunicator)) as TSSCommunicator;
    }

    async Task Update()
    {
        //targetPos = new Vector3(target.position.x, 0, target.position.);

        // Send command for x position and wait for response
        await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 17, 0);
        if (TSS.HasNewData && TSS.LastCommandNumber == 17)
        {
            x = TSS.LastOutputData;
            TSS.setHasNewDataFalse();
        }

        // Send command for y position and wait for response
        await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 18, 0);
        if (TSS.HasNewData && TSS.LastCommandNumber == 18)
        {
            y = TSS.LastOutputData;
            TSS.setHasNewDataFalse();
        }


        //myPos = new Vector3(x, y, transform.position.z);
        myPos = new Vector3(x, y, 0);
        //Debug.Log($"Position updated - x: {x}, y: {y}");

        Vector3 direction = waypointPos - myPos;
        Vector3 upwards = Vector3.forward;
        Quaternion rotation = Quaternion.LookRotation(direction, upwards);
        print(rotation);
    }
}
