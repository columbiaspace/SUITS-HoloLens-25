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

public class vitals_display : MonoBehaviour
{
    // public TSS_DATA TSS; // OLD: Reference to old data handler
    public BackendConnector backendConnector; // NEW: Reference to the backend connector

    public TMP_Text title;
    
    // --- EVA1 Display Fields (Most data points are NOT available in current backend /all endpoint) ---
    public TMP_Text display_batt_time_left_eva1;         // Not Available
    public TMP_Text display_oxy_pri_storage_eva1;      // Not Available
    public TMP_Text display_oxy_sec_storage_eva1;      // Not Available
    public TMP_Text display_oxy_time_left_eva1;        // Not Available
    public TMP_Text display_heart_rate_eva1;           // Not Available
    public TMP_Text display_oxy_consumption_eva1;      // Not Available
    public TMP_Text display_co2_production_eva1;       // Not Available
    public TMP_Text display_suit_pressure_oxy_eva1;    // Not Available
    public TMP_Text display_suit_pressure_co2_eva1;    // Not Available
    public TMP_Text display_suit_pressure_other_eva1;  // Not Available
    public TMP_Text display_suit_pressure_total_eva1;  // Not Available
    public TMP_Text display_fan_pri_rpm_eva1;          // Not Available (DCU provides fan switch state, not RPM)
    public TMP_Text display_fan_sec_rpm_eva1;          // Not Available
    public TMP_Text display_helmet_pressure_co2_eva1;  // Not Available
    public TMP_Text display_scrubber_a_co2_storage_eva1; // Not Available
    public TMP_Text display_scrubber_b_co2_storage_eva1; // Not Available
    public TMP_Text display_temperature_eva1;          // Not Available
    public TMP_Text display_coolant_ml_eva1;           // Not Available
    public TMP_Text display_coolant_gas_pressure_eva1; // Not Available
    public TMP_Text display_coolant_liquid_pressure_eva1; // Not Available

    // --- EVA2 Display Fields (Most data points are NOT available in current backend /all endpoint) ---
    public TMP_Text display_batt_time_left_eva2;         // Not Available
    public TMP_Text display_oxy_pri_storage_eva2;      // Not Available
    public TMP_Text display_oxy_sec_storage_eva2;      // Not Available
    public TMP_Text display_oxy_time_left_eva2;        // Not Available
    public TMP_Text display_heart_rate_eva2;           // Not Available
    public TMP_Text display_oxy_consumption_eva2;      // Not Available
    public TMP_Text display_co2_production_eva2;       // Not Available
    public TMP_Text display_suit_pressure_oxy_eva2;    // Not Available
    public TMP_Text display_suit_pressure_co2_eva2;    // Not Available
    public TMP_Text display_suit_pressure_other_eva2;  // Not Available
    public TMP_Text display_suit_pressure_total_eva2;  // Not Available
    public TMP_Text display_fan_pri_rpm_eva2;          // Not Available (DCU provides fan switch state, not RPM)
    public TMP_Text display_fan_sec_rpm_eva2;          // Not Available
    public TMP_Text display_helmet_pressure_co2_eva2;  // Not Available
    public TMP_Text display_scrubber_a_co2_storage_eva2; // Not Available
    public TMP_Text display_scrubber_b_co2_storage_eva2; // Not Available
    public TMP_Text display_temperature_eva2;          // Not Available
    public TMP_Text display_coolant_ml_eva2;           // Not Available
    public TMP_Text display_coolant_gas_pressure_eva2; // Not Available
    public TMP_Text display_coolant_liquid_pressure_eva2; // Not Available

    private const string N_A = "N/A"; // Placeholder for unavailable data

    void Start()
    {
        // Ensure backendConnector is assigned in the inspector
        if (backendConnector == null)
        {
            Debug.LogError("BackendConnector not assigned in vitals_display script!");
            enabled = false; // Disable script if connector is missing
        }
         // Initialize text fields
        SetAllFields(N_A);
        title.text = "Vitals"; // Default title
    }

    void Update()
    {
        if (backendConnector != null && backendConnector.IsConnected && backendConnector.LatestData != null)
        {
            var data = backendConnector.LatestData;

            // --- Update EVA1 Fields --- 
            // Most fields are unavailable in the current /all endpoint data
            display_batt_time_left_eva1.text = $"Battery time left: {N_A}";
            display_oxy_pri_storage_eva1.text = $"Primary oxygen storage: {N_A}";
            display_oxy_sec_storage_eva1.text = $"Secondary oxygen storage: {N_A}";
            display_oxy_time_left_eva1.text = $"Oxygen time left: {N_A}";
            display_heart_rate_eva1.text = $"Heart rate: {N_A}";
            display_oxy_consumption_eva1.text = $"Oxygen consumption: {N_A}";
            display_co2_production_eva1.text = $"CO2 production: {N_A}";
            display_suit_pressure_oxy_eva1.text = $"Suit oxygen pressure: {N_A}";
            display_suit_pressure_co2_eva1.text = $"Suit CO2 pressure: {N_A}";
            display_suit_pressure_other_eva1.text = $"Suit other pressure: {N_A}";
            display_suit_pressure_total_eva1.text = $"Suit total pressure: {N_A}";
            display_fan_pri_rpm_eva1.text = $"Primary fan rpm: {N_A}";
            display_fan_sec_rpm_eva1.text = $"Secondary fan rpm: {N_A}";
            display_helmet_pressure_co2_eva1.text = $"Helmet CO2 pressure: {N_A}";
            display_scrubber_a_co2_storage_eva1.text = $"Scrubber A CO2 storage: {N_A}";
            display_scrubber_b_co2_storage_eva1.text = $"Scrubber B CO2 storage: {N_A}";
            display_temperature_eva1.text = $"Temperature: {N_A}";
            display_coolant_ml_eva1.text = $"Coolant ml: {N_A}";
            display_coolant_gas_pressure_eva1.text = $"Coolant gas pressure: {N_A}";
            display_coolant_liquid_pressure_eva1.text = $"Coolant liquid pressure: {N_A}";

            // EXAMPLE: Accessing AVAILABLE data (if you had UI elements for them)
            // if (data.eva1?.dcu != null)
            // {
            //      // eva1_dcu_battery_status_text.text = $"EVA1 Batt Switch: {data.eva1.dcu.battery}"; 
            //      // eva1_dcu_fan_status_text.text = $"EVA1 Fan Switch: {data.eva1.dcu.fan}"; 
            // }
            // if (data.eva1?.imu != null)
            // {
            //      // eva1_imu_position_text.text = $"EVA1 Pos: ({data.eva1.imu.posx:F2}, {data.eva1.imu.posy:F2})";
            // }

            // --- Update EVA2 Fields ---
            display_batt_time_left_eva2.text = $"Battery time left: {N_A}";
            display_oxy_pri_storage_eva2.text = $"Primary oxygen storage: {N_A}";
            display_oxy_sec_storage_eva2.text = $"Secondary oxygen storage: {N_A}";
            display_oxy_time_left_eva2.text = $"Oxygen time left: {N_A}";
            display_heart_rate_eva2.text = $"Heart rate: {N_A}";
            display_oxy_consumption_eva2.text = $"Oxygen consumption: {N_A}";
            display_co2_production_eva2.text = $"CO2 production: {N_A}";
            display_suit_pressure_oxy_eva2.text = $"Suit oxygen pressure: {N_A}";
            display_suit_pressure_co2_eva2.text = $"Suit CO2 pressure: {N_A}";
            display_suit_pressure_other_eva2.text = $"Suit other pressure: {N_A}";
            display_suit_pressure_total_eva2.text = $"Suit total pressure: {N_A}";
            display_fan_pri_rpm_eva2.text = $"Primary fan rpm: {N_A}";
            display_fan_sec_rpm_eva2.text = $"Secondary fan rpm: {N_A}";
            display_helmet_pressure_co2_eva2.text = $"Helmet CO2 pressure: {N_A}";
            display_scrubber_a_co2_storage_eva2.text = $"Scrubber A CO2 storage: {N_A}";
            display_scrubber_b_co2_storage_eva2.text = $"Scrubber B CO2 storage: {N_A}";
            display_temperature_eva2.text = $"Temperature: {N_A}";
            display_coolant_ml_eva2.text = $"Coolant ml: {N_A}";
            display_coolant_gas_pressure_eva2.text = $"Coolant gas pressure: {N_A}";
            display_coolant_liquid_pressure_eva2.text = $"Coolant liquid pressure: {N_A}";

             // EXAMPLE: Accessing AVAILABLE EVA2 data
            // if (data.eva2?.dcu != null)
            // {
            //      // eva2_dcu_battery_status_text.text = $"EVA2 Batt Switch: {data.eva2.dcu.battery}"; 
            // }
        }
        else
        {
            // Handle disconnected state - show placeholders or error
            string statusMessage = backendConnector == null ? "No Connector" : "Connecting...";
            if (backendConnector != null && !string.IsNullOrEmpty(backendConnector.LastError))
            {
                statusMessage = $"Error: {backendConnector.LastError}";
                // Potentially display the error message in the title or a dedicated status field
                // title.text = statusMessage; 
            }
            SetAllFields(statusMessage == "Connecting..." ? "..." : "Err"); // Show ... or Err 
        }
    }

    // Helper to set all display fields to a specific text
    private void SetAllFields(string text)
    {
        if (display_batt_time_left_eva1 != null) display_batt_time_left_eva1.text = $"Battery time left: {text}";
        if (display_oxy_pri_storage_eva1 != null) display_oxy_pri_storage_eva1.text = $"Primary oxygen storage: {text}";
        if (display_oxy_sec_storage_eva1 != null) display_oxy_sec_storage_eva1.text = $"Secondary oxygen storage: {text}";
        if (display_oxy_time_left_eva1 != null) display_oxy_time_left_eva1.text = $"Oxygen time left: {text}";
        if (display_heart_rate_eva1 != null) display_heart_rate_eva1.text = $"Heart rate: {text}";
        if (display_oxy_consumption_eva1 != null) display_oxy_consumption_eva1.text = $"Oxygen consumption: {text}";
        if (display_co2_production_eva1 != null) display_co2_production_eva1.text = $"CO2 production: {text}";
        if (display_suit_pressure_oxy_eva1 != null) display_suit_pressure_oxy_eva1.text = $"Suit oxygen pressure: {text}";
        if (display_suit_pressure_co2_eva1 != null) display_suit_pressure_co2_eva1.text = $"Suit CO2 pressure: {text}";
        if (display_suit_pressure_other_eva1 != null) display_suit_pressure_other_eva1.text = $"Suit other pressure: {text}";
        if (display_suit_pressure_total_eva1 != null) display_suit_pressure_total_eva1.text = $"Suit total pressure: {text}";
        if (display_fan_pri_rpm_eva1 != null) display_fan_pri_rpm_eva1.text = $"Primary fan rpm: {text}";
        if (display_fan_sec_rpm_eva1 != null) display_fan_sec_rpm_eva1.text = $"Secondary fan rpm: {text}";
        if (display_helmet_pressure_co2_eva1 != null) display_helmet_pressure_co2_eva1.text = $"Helmet CO2 pressure: {text}";
        if (display_scrubber_a_co2_storage_eva1 != null) display_scrubber_a_co2_storage_eva1.text = $"Scrubber A CO2 storage: {text}";
        if (display_scrubber_b_co2_storage_eva1 != null) display_scrubber_b_co2_storage_eva1.text = $"Scrubber B CO2 storage: {text}";
        if (display_temperature_eva1 != null) display_temperature_eva1.text = $"Temperature: {text}";
        if (display_coolant_ml_eva1 != null) display_coolant_ml_eva1.text = $"Coolant ml: {text}";
        if (display_coolant_gas_pressure_eva1 != null) display_coolant_gas_pressure_eva1.text = $"Coolant gas pressure: {text}";
        if (display_coolant_liquid_pressure_eva1 != null) display_coolant_liquid_pressure_eva1.text = $"Coolant liquid pressure: {text}";

        if (display_batt_time_left_eva2 != null) display_batt_time_left_eva2.text = $"Battery time left: {text}";
        if (display_oxy_pri_storage_eva2 != null) display_oxy_pri_storage_eva2.text = $"Primary oxygen storage: {text}";
        if (display_oxy_sec_storage_eva2 != null) display_oxy_sec_storage_eva2.text = $"Secondary oxygen storage: {text}";
        if (display_oxy_time_left_eva2 != null) display_oxy_time_left_eva2.text = $"Oxygen time left: {text}";
        if (display_heart_rate_eva2 != null) display_heart_rate_eva2.text = $"Heart rate: {text}";
        if (display_oxy_consumption_eva2 != null) display_oxy_consumption_eva2.text = $"Oxygen consumption: {text}";
        if (display_co2_production_eva2 != null) display_co2_production_eva2.text = $"CO2 production: {text}";
        if (display_suit_pressure_oxy_eva2 != null) display_suit_pressure_oxy_eva2.text = $"Suit oxygen pressure: {text}";
        if (display_suit_pressure_co2_eva2 != null) display_suit_pressure_co2_eva2.text = $"Suit CO2 pressure: {text}";
        if (display_suit_pressure_other_eva2 != null) display_suit_pressure_other_eva2.text = $"Suit other pressure: {text}";
        if (display_suit_pressure_total_eva2 != null) display_suit_pressure_total_eva2.text = $"Suit total pressure: {text}";
        if (display_fan_pri_rpm_eva2 != null) display_fan_pri_rpm_eva2.text = $"Primary fan rpm: {text}";
        if (display_fan_sec_rpm_eva2 != null) display_fan_sec_rpm_eva2.text = $"Secondary fan rpm: {text}";
        if (display_helmet_pressure_co2_eva2 != null) display_helmet_pressure_co2_eva2.text = $"Helmet CO2 pressure: {text}";
        if (display_scrubber_a_co2_storage_eva2 != null) display_scrubber_a_co2_storage_eva2.text = $"Scrubber A CO2 storage: {text}";
        if (display_scrubber_b_co2_storage_eva2 != null) display_scrubber_b_co2_storage_eva2.text = $"Scrubber B CO2 storage: {text}";
        if (display_temperature_eva2 != null) display_temperature_eva2.text = $"Temperature: {text}";
        if (display_coolant_ml_eva2 != null) display_coolant_ml_eva2.text = $"Coolant ml: {text}";
        if (display_coolant_gas_pressure_eva2 != null) display_coolant_gas_pressure_eva2.text = $"Coolant gas pressure: {text}";
        if (display_coolant_liquid_pressure_eva2 != null) display_coolant_liquid_pressure_eva2.text = $"Coolant liquid pressure: {text}";
    }

    // These methods likely switch which EVA's data is primarily displayed 
    // or just set the title. They don't need changing unless the UI logic changes.
    public void eva1(){
        title.text = "EVA1 Vitals";
    }

    public void eva2(){
        title.text = "EVA2 Vitals";
    }
}
