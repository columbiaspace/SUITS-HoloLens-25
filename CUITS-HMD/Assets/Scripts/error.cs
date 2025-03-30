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
//using static TSS_Serialized.UIAData;
using System;
using System.Xml;


public class error : MonoBehaviour
{
    // public TSS_DATA TSS; // OLD: Reference to old data handler
    public BackendConnector backendConnector; // NEW: Reference to the backend connector
    public TMP_Text display;

    // Optional: Define thresholds for errors if needed (assuming 1.0 means active error)
    private const float ERROR_THRESHOLD = 0.9f; 

    // Public flag to control error checking (can be set by other scripts/events)
    public bool duringEVA = false; 

    // Start is called before the first frame update
    void Start()
    {
        if (backendConnector == null)
        {
            Debug.LogError("BackendConnector not assigned in error script!");
            enabled = false; 
        }
        if (display == null)
        {
             Debug.LogError("Display TMP_Text not assigned in error script!");
             enabled = false;
        }
         else
        {
            display.text = "Awaiting Connection...";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (backendConnector == null || display == null) return; // Should have been disabled in Start

        if (backendConnector.IsConnected && backendConnector.LatestData != null)
        { 
            string errorMsg = ""; // Accumulate error messages

            if (duringEVA) // Only check for errors if EVA is active
            {
                var data = backendConnector.LatestData;

                // --- Check AVAILABLE error flags --- 
                if (data.errors != null)
                {
                    if (data.errors.error1 > ERROR_THRESHOLD) errorMsg += "Error State 1 Active!\n";
                    if (data.errors.error2 > ERROR_THRESHOLD) errorMsg += "Error State 2 Active!\n";
                    if (data.errors.error3 > ERROR_THRESHOLD) errorMsg += "Error State 3 Active!\n";
                    // Add more specific messages based on what error1, error2, error3 mean
                }
                else
                {
                    // This case should ideally not happen if backend sends the structure correctly
                     errorMsg += "Error data structure missing.\n";
                }

                // --- Existing checks based on UNAVAILABLE data (Commented Out) ---
                /*
                // heart_rate
                if (TSS.tel.telemetry.eva1.heart_rate > 160)
                {
                    errorMsg += "Detected heart rate too high: please slow down\n";
                }

                // suit_pressure_oxy
                if (TSS.tel.telemetry.eva1.suit_pressure_oxy < 3.5 || TSS.tel.telemetry.eva1.suit_pressure_oxy > 4.1)
                {
                    errorMsg += "Swap to secondary oxygen tank\n";
                }

                // suit_pressure_co2
                if (TSS.tel.telemetry.eva1.suit_pressure_co2 > 0.1)
                {
                     errorMsg += "Scrubber has filled up and must be vented, flip DCU CO2 switch\n";
                     // Potential future check: Link this to data.eva1.dcu.co2 if that switch state indicates venting needed
                }
                
                // suit_pressure_other
                if (TSS.tel.telemetry.eva1.suit_pressure_other > 0.5)
                {
                    errorMsg += "Partial pressure of all gases are not zero\n";
                }

                // suit_pressure_total & related checks
                if (TSS.tel.telemetry.eva1.suit_pressure_total < 3.5 || TSS.tel.telemetry.eva1.suit_pressure_total > 4.5)
                {
                     // Combined errors often need specific logic not replicable without full data
                     errorMsg += "Suit total pressure out of range\n";
                }

                // helmet_pressure_co2
                if (TSS.tel.telemetry.eva1.helmet_pressure_co2 > 0.15)
                {
                     errorMsg += "Swap to secondary fan\n";
                     // Potential future check: Link this to data.eva1.dcu.fan state if primary is off/faulty
                }

                // fan_pri_rpm and fan_sec_rpm
                // ... (Requires RPM data, not just switch state)

                // scrubber_a_co2_storage and scrubber_b_co2_storage
                if (TSS.tel.telemetry.eva1.scrubber_a_co2_storage > 60 || TSS.tel.telemetry.eva1.scrubber_b_co2_storage > 60)
                {
                    errorMsg += "Vent collected carbon dioxide, flip DCU CO2 switch\n";
                }

                // temperature
                if (TSS.tel.telemetry.eva1.temperature > 90)
                {
                    errorMsg += "Detected temperature too high: please slow down\n";
                }
                */
            }
            else // Not during EVA
            {
                 errorMsg = ""; // Clear errors when not in EVA
            }

            // Update display
            if (string.IsNullOrEmpty(errorMsg)) 
            {
                 display.text = duringEVA ? "Status: OK" : "Status: Idle"; // Show OK or Idle
            }
            else
            {
                 display.text = errorMsg.TrimEnd(); // Display accumulated errors
            }
        }
        else // Not connected or initialising
        {
            display.text = backendConnector.IsConnected ? "Waiting for data..." : "Connecting...";
            if (!string.IsNullOrEmpty(backendConnector.LastError))
            {
                display.text += $"\nError: {backendConnector.LastError}";
            }
        }
    }
}
