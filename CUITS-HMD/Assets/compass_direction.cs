using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compass_direction : MonoBehaviour
{
    public TSS_DATA tss;
    public GameObject Compass;
    float curr_z_rot;
    public float actual_heading;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        actual_heading = (float)(tss.imu.dcu.eva1.heading);
        if(actual_heading < 0.0f){
            actual_heading += 360.0f;
        }
        Compass.transform.Rotate(0.0f, 0.0f, (curr_z_rot - actual_heading), Space.Self);
        curr_z_rot =  actual_heading;
    }
}
