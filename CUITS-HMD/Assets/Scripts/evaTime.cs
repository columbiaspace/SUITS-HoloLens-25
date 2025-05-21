using UnityEngine;
using TMPro;
using System;

public class EvaTime : MonoBehaviour
{
    public TMP_Text display; // Assign this in Unity Inspector
    // public bool showHoursMinutesSeconds = true; // Format as HH:MM:SS vs raw seconds - Will always be true now
    
    // private float elapsedTime = 0f; // Removed
    // private double lastTimestamp = 0; // Removed
    // private float lastInternalUpdateTime = 0f; // Removed
    
    void Start()
    {
        if (display == null)
        {
            Debug.LogError("EvaTime: Text display not assigned in inspector!");
            enabled = false;
            return;
        }
        
        display.text = "00:00"; // Initial display
        // Debug.Log("EvaTime: Started"); // Optional: remove debug log for cleaner console
    }

    void Update()
    {
        var backendService = BackendDataService.Instance;
        
        if (backendService != null && backendService.LatestData != null)
        {
            float currentEvaTime = backendService.LatestData.eva_time;
            UpdateTimeDisplay(currentEvaTime);
            // Debug.Log($"EvaTime: Displaying backend time {currentEvaTime}"); // Optional: for debugging
        }
        else
        {
            if (backendService == null)
            { 
                display.text = "ERR: No Backend";
                // Debug.LogError("EvaTime: BackendDataService not found in scene!"); // Optional
            }
            else // backendService is not null, but LatestData is
            {
                display.text = "Connecting...";
            }
        }
    }
    
    private void UpdateTimeDisplay(float seconds)
    {
        // Format as HH:MM:SS
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        
        if (timeSpan.Hours > 0)
        {
            display.text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else
        {
            display.text = string.Format("{0:D2}:{1:D2}", 
                timeSpan.Minutes, timeSpan.Seconds);
        }
        // Removed the 'else' branch for raw seconds as per new requirement
    }
}
