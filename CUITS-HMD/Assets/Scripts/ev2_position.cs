using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ev2_position : MonoBehaviour
{
    private Vector3 initialPosition;
    private bool hasReceivedPosition = false;
    

    void Start()
    {

        // Store the initial position when the script starts
        initialPosition = transform.localPosition;
        Debug.Log($"[ev2_position] Start - Initial position: {initialPosition}");
        

        // Check if APIClient exists
        if (FindObjectOfType<APIClient>() == null)
        {
            Debug.LogError("[ev2_position] ERROR: No APIClient found in scene! Please add APIClient component to a GameObject.");
        }
    }

    void Update()
    {
        // Check if we have a valid position from APIClient
        if (APIClient.LatestPosition2 != Vector3.zero)
        {
            if (!hasReceivedPosition)
            {
                Debug.Log($"[ev2_position] First position received: {APIClient.LatestPosition2}");
                hasReceivedPosition = true;
            }

            // Use the static LatestPosition from APIClient
            Vector3 targetPosition = APIClient.LatestPosition2;
            
            // Log the positions for debugging
            Debug.Log($"[ev2_position] Current: {transform.localPosition}, Target: {targetPosition}");

            // Move the GameObject this script is attached to
            transform.localPosition = targetPosition;
        }
        else
        {
            Debug.LogWarning("[ev2_position] Waiting for valid position from APIClient...");
        }
    }
}
