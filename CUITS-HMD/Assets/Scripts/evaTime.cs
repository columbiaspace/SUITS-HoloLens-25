using UnityEngine;
using TMPro;
using System;

public class EvaTime : MonoBehaviour
{
    public TMP_Text display; // Assign this in Unity Inspector
    public bool showHoursMinutesSeconds = true; // Format as HH:MM:SS vs raw seconds
    
    private float elapsedTime = 0f;
    private double lastTimestamp = 0;
    private float lastInternalUpdateTime = 0f;
    
    void Start()
    {
        if (display == null)
        {
            Debug.LogError("EvaTime: Text display not assigned in inspector!");
            enabled = false;
            return;
        }
        
        display.text = "00:00"; // Initial display
        Debug.Log("EvaTime: Started");
    }

    void Update()
    {
        // Update time from backend data if available
        if (BackendDataService.Instance != null && BackendDataService.Instance.LatestData != null)
        {
            float currentEvaTime = BackendDataService.Instance.LatestData.eva_time;
            double currentTimestamp = BackendDataService.Instance.LatestData.last_updated;
            
            // Only update display if we got new data
            if (currentTimestamp > lastTimestamp)
            {
                lastTimestamp = currentTimestamp;
                lastInternalUpdateTime = Time.time;
                elapsedTime = currentEvaTime;
                
                UpdateTimeDisplay(elapsedTime);
                Debug.Log($"EvaTime: Updated from backend to {elapsedTime}");
            }
            else
            {
                // Smoothly increment timer locally between backend updates
                float timeSinceLastUpdate = Time.time - lastInternalUpdateTime;
                if (timeSinceLastUpdate > 0.5f) // Only update display every 0.5 seconds
                {
                    elapsedTime += timeSinceLastUpdate;
                    lastInternalUpdateTime = Time.time;
                    UpdateTimeDisplay(elapsedTime);
                }
            }
        }
        else
        {
            // Handle case where data isn't available yet
            if (BackendDataService.Instance == null)
            {
                display.text = "ERR: No Backend";
                Debug.LogError("EvaTime: BackendDataService not found in scene!");
            }
            else
            {
                display.text = "Connecting...";
            }
        }
    }
    
    private void UpdateTimeDisplay(float seconds)
    {
        if (showHoursMinutesSeconds)
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
        }
        else
        {
            // Just display raw seconds rounded to nearest whole number
            display.text = Mathf.RoundToInt(seconds).ToString();
        }
    }
}


// TimeResponse class is used to deserialize the JSON response from the FastAPI server
[System.Serializable] // Makes the class serializable so JsonUtility can turn JSON into this object
public class TimeResponse
{
    // Fields should match the JSON keys returned by FastAPI
    public string eva_time;
}
