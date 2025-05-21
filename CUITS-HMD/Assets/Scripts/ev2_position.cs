using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ev2_position : MonoBehaviour
{
    private Vector3 initialPosition;
    private bool hasReceivedPosition = false;
    
    // Scale and offset values from the original APIClient conversion
    private const float SCALE_FACTOR = 0.162f;
    private const float OFFSET_X = 916.52f;
    private const float OFFSET_Y = 1619.58f;
    private const float Z_POS = -0.5f;

    void Start()
    {
        // Store the initial position when the script starts
        initialPosition = transform.localPosition;
        Debug.Log($"[ev2_position] Start - Initial position: {initialPosition}");

        // Check if BackendDataService exists
        if (BackendDataService.Instance == null)
        {
            Debug.LogError("[ev2_position] ERROR: No BackendDataService found in scene! Please add BackendDataService component to a GameObject.");
        }
    }

    void Update()
    {
        // Get position from BackendDataService instead of APIClient
        if (BackendDataService.Instance != null && 
            BackendDataService.Instance.LatestData != null && 
            BackendDataService.Instance.LatestData.eva2 != null && 
            BackendDataService.Instance.LatestData.eva2.imu != null)
        {
            float posx = BackendDataService.Instance.LatestData.eva2.imu.posx ;
            float posy = BackendDataService.Instance.LatestData.eva2.imu.posy ;
            
            // Only update if we have valid position data
            if (posx != 0 || posy != 0) 
            {
                if (!hasReceivedPosition)
                {
                    Debug.Log($"[ev2_position] First position received: posx={posx}, posy={posy}");
                    hasReceivedPosition = true;
                }
                
                // Convert coordinates using the same formula as in the original APIClient
                Vector3 targetPosition = new Vector3(SCALE_FACTOR * posx + OFFSET_X, SCALE_FACTOR * posy + OFFSET_Y, Z_POS);
                
                // Move the GameObject this script is attached to
                transform.localPosition = targetPosition;
            }
        }
        else
        {
            Debug.LogWarning("[ev2_position] Waiting for valid position from BackendDataService...");
        }
    }
}
