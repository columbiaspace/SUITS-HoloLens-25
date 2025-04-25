using System;
using System.Threading.Tasks;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    // Target to point towards
    public Transform target;
    public Vector3 targetPos;
    public Vector3 myPos;

    private TSSCommunicator TSS;

    private float x;
    private float y;

    

    void Start(){
        TSS = FindObjectOfType(typeof(TSSCommunicator)) as TSSCommunicator;
    }

    async Task Update()
    {
        targetPos = target.position;
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

        // testing the TSS
        await TSS.SendCommand((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 61, 1);
        if (TSS.HasNewData && TSS.LastCommandNumber == 61)
        {
            print("test");
            print("data " + TSS.LastOutputData);
            TSS.setHasNewDataFalse();
        }

        //myPos = new Vector3(x, y, transform.position.z);
        myPos = new Vector3(x, transform.position.y, y);
        transform.position = myPos;
        //Debug.Log($"Position updated - x: {x}, y: {y}");

        if (target != null)
        {
            Vector3 direction = targetPos - myPos;
            Vector3 upwards = Vector3.forward;
            Quaternion rotation = Quaternion.LookRotation(direction, upwards);
            transform.rotation = rotation;
        }
    }
}
