using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Profiling;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using Newtonsoft.Json;


using TMPro;
using Unity.VisualScripting;
using System;
using System.Xml;

public class evaTime : MonoBehaviour
{
   // public TSS_DATA TSS; // OLD: Reference to old data handler
   // public TSScConnection TSSc; // OLD: Reference to old connection
   public BackendConnector backendConnector; // NEW: Reference to the backend connector
   public TMP_Text display; 
   public TMP_Text check; // Keep this if it displays connection status or similar

   // --- Variables for time calculation --- 
   int minutes;
   int seconds;
   int time_in_secs; 

    // Start is called before the first frame update
    void Start()
    {
        if (backendConnector == null)
        {
            Debug.LogError("BackendConnector not assigned in evaTime script!");
            enabled = false; 
        }
         if (display == null) 
        {
            Debug.LogError("Display TMP_Text not assigned in evaTime script!");
            enabled = false; 
        }
         else 
        {
            display.text = "--:--"; // Initial placeholder
        }
        if (check != null) check.text = "Connecting...";
    }

    // Update is called once per frame
    void Update()
    {
       if (backendConnector != null && backendConnector.IsConnected && backendConnector.LatestData != null)
       {
            // Access eva_time from the updated structure
            time_in_secs = (int)backendConnector.LatestData.eva_time; 

            // Prevent negative time display if eva_time is not yet valid (e.g., -1 from backend)
            if (time_in_secs < 0) time_in_secs = 0;

            minutes = time_in_secs / 60;
            seconds = time_in_secs % 60;
            
            // Format the display string H:MM:SS (or M:SS if minutes < 60) - adjust if needed
            display.text = string.Format("{0}:{1:00}", minutes, seconds); 
            
            if (check != null) check.text = "Connected"; 
       }
       else
       {
            display.text = "--:--";
            if (check != null) 
            {
                check.text = backendConnector != null ? "Connecting..." : "No Connector";
                if (backendConnector != null && !string.IsNullOrEmpty(backendConnector.LastError))
                {
                    // Append error concisely
                    check.text = $"Error: {backendConnector.LastError.Split('\n')[0]}"; 
                }
            }
       }
    }
}
