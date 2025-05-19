using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ev1_position : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust for smoothness

    void Update()
    {
        Vector3 targetPosition = APIClient.LatestPosition;

        // Move the GameObject this script is attached to
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}