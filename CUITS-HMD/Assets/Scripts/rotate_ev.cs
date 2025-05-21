using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_ev : MonoBehaviour
{
    float curr_z_rot;
    public float actual_heading; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var backend = BackendDataService.Instance;
        if (backend == null || backend.LatestData == null || backend.LatestData.eva1 == null)
        {
             Debug.LogWarning("IMU data not available from backend.");
             return;
        }
        actual_heading = (float)(backend.LatestData.eva1.imu.heading);
        if(actual_heading < 0.0f){
           actual_heading += 360.0f;
        }
        // Simply set the absolute rotation instead of calculating differences - from cursour
        transform.localRotation = Quaternion.Euler(0, 0, actual_heading);
        
    }
}
