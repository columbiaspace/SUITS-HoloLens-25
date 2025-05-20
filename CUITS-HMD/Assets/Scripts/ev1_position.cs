using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ev1_position : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust for smoothness
    private Vector3 initialPosition;
    private bool hasReceivedPosition = false;
    

    void Start()
    {

        // Store the initial position when the script starts
        initialPosition = transform.localPosition;
        Debug.Log($"[ev1_position] Start - Initial position: {initialPosition}");
        

        // Check if APIClient exists
        if (FindObjectOfType<APIClient>() == null)
        {
            Debug.LogError("[ev1_position] ERROR: No APIClient found in scene! Please add APIClient component to a GameObject.");
        }
    }

    void Update()
    {
        // Check if we have a valid position from APIClient
        if (APIClient.LatestPosition != Vector3.zero)
        {
            if (!hasReceivedPosition)
            {
                Debug.Log($"[ev1_position] First position received: {APIClient.LatestPosition}");
                hasReceivedPosition = true;
            }

            // Use the static LatestPosition from APIClient
            Vector3 targetPosition = APIClient.LatestPosition;
            
            // Log the positions for debugging
            Debug.Log($"[ev1_position] Current: {transform.localPosition}, Target: {targetPosition}, Speed: {moveSpeed}");

            // Move the GameObject this script is attached to
            transform.localPosition = targetPosition;
        }
        else
        {
            Debug.LogWarning("[ev1_position] Waiting for valid position from APIClient...");
        }
    }
}
