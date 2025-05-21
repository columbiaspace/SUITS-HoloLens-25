using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertCoords : MonoBehaviour
{
    public GameObject pin;

    double uniposx;
    double uniposy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if BackendDataService is available with data
        if (BackendDataService.Instance != null && 
            BackendDataService.Instance.LatestData != null && 
            BackendDataService.Instance.LatestData.eva1 != null && 
            BackendDataService.Instance.LatestData.eva1.imu != null)
        {
            float posx = BackendDataService.Instance.LatestData.eva1.imu.posx;
            float posy = BackendDataService.Instance.LatestData.eva1.imu.posy;
            
            // Use the same conversion formula but with data from BackendDataService
            uniposx = .00293 + (.0030824 * (posx - 298355));
            uniposy = .04249 + (.0026632407 * (posy - 3272383));
            
            pin.transform.position = new Vector3((float)uniposx, (float)uniposy, 0);
        }
    }
}
