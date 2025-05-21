using System.Collections;
// using System.Collections.Generic; // No longer needed for local data structures
using UnityEngine;
using TMPro;
// using UnityEngine.Networking; // No longer needed for direct API calls
using System;

public class VitalsDisplay : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text display_batt_time_left_eva1;
    public TMP_Text display_oxy_pri_storage_eva1;
    public TMP_Text display_oxy_sec_storage_eva1;
    public TMP_Text display_oxy_time_left_eva1;
    public TMP_Text display_heart_rate_eva1;
    public TMP_Text display_oxy_consumption_eva1;
    public TMP_Text display_co2_production_eva1;
    public TMP_Text display_suit_pressure_oxy_eva1;
    public TMP_Text display_suit_pressure_co2_eva1;
    public TMP_Text display_suit_pressure_other_eva1;
    public TMP_Text display_suit_pressure_total_eva1;
    public TMP_Text display_fan_pri_rpm_eva1;
    public TMP_Text display_fan_sec_rpm_eva1;
    public TMP_Text display_helmet_pressure_co2_eva1;
    public TMP_Text display_scrubber_a_co2_storage_eva1;
    public TMP_Text display_scrubber_b_co2_storage_eva1;
    public TMP_Text display_temperature_eva1;
    public TMP_Text display_coolant_ml_eva1;
    public TMP_Text display_coolant_gas_pressure_eva1;
    public TMP_Text display_coolant_liquid_pressure_eva1;

    public TMP_Text display_batt_time_left_eva2;
    public TMP_Text display_oxy_pri_storage_eva2;
    public TMP_Text display_oxy_sec_storage_eva2;
    public TMP_Text display_oxy_time_left_eva2;
    public TMP_Text display_heart_rate_eva2;
    public TMP_Text display_oxy_consumption_eva2;
    public TMP_Text display_co2_production_eva2;
    public TMP_Text display_suit_pressure_oxy_eva2;
    public TMP_Text display_suit_pressure_co2_eva2;
    public TMP_Text display_suit_pressure_other_eva2;
    public TMP_Text display_suit_pressure_total_eva2;
    public TMP_Text display_fan_pri_rpm_eva2;
    public TMP_Text display_fan_sec_rpm_eva2;
    public TMP_Text display_helmet_pressure_co2_eva2;
    public TMP_Text display_scrubber_a_co2_storage_eva2;
    public TMP_Text display_scrubber_b_co2_storage_eva2;
    public TMP_Text display_temperature_eva2;
    public TMP_Text display_coolant_ml_eva2;
    public TMP_Text display_coolant_gas_pressure_eva2;
    public TMP_Text display_coolant_liquid_pressure_eva2;

    [Header("Optional Status Display")]
    public TMP_Text statusText; // To show data freshness and connection status

    private double lastUpdateTimestamp = 0;
    private int updateCounter = 0;
    private float timeSinceLastSuccessfulUpdate = 0f;

    void Start()
    {
        Debug.Log("VitalsDisplay: Started");
        // Set initial status
        if (title != null) title.text = "EVA Vitals - Waiting for data...";
        if (statusText != null) statusText.text = "Connecting...";
        
        // Set all displays to "Waiting"
        SetAllDisplaysToDefault("Waiting for data...");
    }

    void Update()
    {
        timeSinceLastSuccessfulUpdate += Time.deltaTime;
        
        if (BackendDataService.Instance != null && BackendDataService.Instance.LatestData != null)
        {
            AllDataResponse data = BackendDataService.Instance.LatestData;

            // Check if data is fresh by comparing timestamps
            if (data.last_updated > lastUpdateTimestamp)
            {
                lastUpdateTimestamp = data.last_updated;
                updateCounter++;
                timeSinceLastSuccessfulUpdate = 0f;
                
                Debug.Log($"VitalsDisplay: Updating data (#{updateCounter}), timestamp: {lastUpdateTimestamp}");
                
                if (title != null) title.text = $"EVA Vitals";
                if (statusText != null) statusText.text = $"Data Updated: {DateTime.Now.ToString("HH:mm:ss")}";
                
                UpdateEVA1Display(data);
                UpdateEVA2Display(data);
            }
            else if (timeSinceLastSuccessfulUpdate > 5f) 
            {
                // It's been over 5 seconds with no new data
                if (statusText != null) 
                {
                    statusText.text = $"Data Stale: Last update {timeSinceLastSuccessfulUpdate:F0}s ago";
                    if (timeSinceLastSuccessfulUpdate > 30f)
                    {
                        statusText.text = $"WARNING: Data very stale ({timeSinceLastSuccessfulUpdate:F0}s old)";
                        Debug.LogWarning($"VitalsDisplay: Data is very stale - last update was {timeSinceLastSuccessfulUpdate:F0} seconds ago");
                    }
                }
            }
        }
        else
        {
            if (BackendDataService.Instance == null)
            {
                Debug.LogError("VitalsDisplay: BackendDataService.Instance is null! Add BackendDataService to scene.");
                if (statusText != null) statusText.text = "ERROR: BackendDataService not found!";
            }
            else if (BackendDataService.Instance.LatestData == null)
            {
                if (timeSinceLastSuccessfulUpdate > 10f)
                {
                    Debug.LogError($"VitalsDisplay: No data received for {timeSinceLastSuccessfulUpdate:F0} seconds. Backend may be unreachable.");
                    if (statusText != null) statusText.text = $"ERROR: No data from backend ({timeSinceLastSuccessfulUpdate:F0}s)";
                }
                else 
                {
                    if (statusText != null) statusText.text = "Waiting for first data...";
                }
            }
        }
    }

    private void UpdateEVA1Display(AllDataResponse data)
    {
        if (data.eva1 != null && data.eva1.telemetry != null)
        {
            var telemetryEva1 = data.eva1.telemetry;
            display_batt_time_left_eva1.text = "Battery time left: " + telemetryEva1.batt_time_left.ToString("F1");
            display_oxy_pri_storage_eva1.text = "Primary oxygen storage: " + telemetryEva1.oxy_pri_storage.ToString("F1");
            display_oxy_sec_storage_eva1.text = "Secondary oxygen storage: " + telemetryEva1.oxy_sec_storage.ToString("F1");
            display_oxy_time_left_eva1.text = "Oxygen time left: " + telemetryEva1.oxy_time_left.ToString("F0");
            display_heart_rate_eva1.text = "Heart rate: " + telemetryEva1.heart_rate.ToString("F0");
            display_oxy_consumption_eva1.text = "Oxygen consumption: " + telemetryEva1.oxy_consumption.ToString("F2");
            display_co2_production_eva1.text = "CO2 production: " + telemetryEva1.co2_production.ToString("F2");
            display_suit_pressure_oxy_eva1.text = "Suit pressure O2: " + telemetryEva1.suit_pressure_oxy.ToString("F2");
            display_suit_pressure_co2_eva1.text = "Suit pressure CO2: " + telemetryEva1.suit_pressure_co2.ToString("F2");
            display_suit_pressure_other_eva1.text = "Suit pressure other: " + telemetryEva1.suit_pressure_other.ToString("F2");
            display_suit_pressure_total_eva1.text = "Suit pressure total: " + telemetryEva1.suit_pressure_total.ToString("F2");
            display_fan_pri_rpm_eva1.text = "Fan primary RPM: " + telemetryEva1.fan_pri_rpm.ToString("F0");
            display_fan_sec_rpm_eva1.text = "Fan secondary RPM: " + telemetryEva1.fan_sec_rpm.ToString("F0");
            display_helmet_pressure_co2_eva1.text = "Helmet pressure CO2: " + telemetryEva1.helmet_pressure_co2.ToString("F2");
            display_scrubber_a_co2_storage_eva1.text = "Scrubber A CO2: " + telemetryEva1.scrubber_a_co2_storage.ToString("F1");
            display_scrubber_b_co2_storage_eva1.text = "Scrubber B CO2: " + telemetryEva1.scrubber_b_co2_storage.ToString("F1");
            display_temperature_eva1.text = "Temperature: " + telemetryEva1.temperature.ToString("F1");
            display_coolant_ml_eva1.text = "Coolant ML: " + telemetryEva1.coolant_ml.ToString("F0");
            display_coolant_gas_pressure_eva1.text = "Coolant gas pressure: " + telemetryEva1.coolant_gas_pressure.ToString("F1");
            display_coolant_liquid_pressure_eva1.text = "Coolant liquid pressure: " + telemetryEva1.coolant_liquid_pressure.ToString("F1");
        }
        else
        {
            Debug.LogWarning("VitalsDisplay: EVA1 telemetry data is null");
            SetEVA1DisplaysToDefault("No EVA1 Data");
        }
    }

    private void UpdateEVA2Display(AllDataResponse data)
    {
        if (data.eva2 != null && data.eva2.telemetry != null)
        {
            var telemetryEva2 = data.eva2.telemetry;
            display_batt_time_left_eva2.text = "Battery time left: " + telemetryEva2.batt_time_left.ToString("F1");
            display_oxy_pri_storage_eva2.text = "Primary oxygen storage: " + telemetryEva2.oxy_pri_storage.ToString("F1");
            display_oxy_sec_storage_eva2.text = "Secondary oxygen storage: " + telemetryEva2.oxy_sec_storage.ToString("F1");
            display_oxy_time_left_eva2.text = "Oxygen time left: " + telemetryEva2.oxy_time_left.ToString("F0");
            display_heart_rate_eva2.text = "Heart rate: " + telemetryEva2.heart_rate.ToString("F0");
            display_oxy_consumption_eva2.text = "Oxygen consumption: " + telemetryEva2.oxy_consumption.ToString("F2");
            display_co2_production_eva2.text = "CO2 production: " + telemetryEva2.co2_production.ToString("F2");
            display_suit_pressure_oxy_eva2.text = "Suit pressure O2: " + telemetryEva2.suit_pressure_oxy.ToString("F2");
            display_suit_pressure_co2_eva2.text = "Suit pressure CO2: " + telemetryEva2.suit_pressure_co2.ToString("F2");
            display_suit_pressure_other_eva2.text = "Suit pressure other: " + telemetryEva2.suit_pressure_other.ToString("F2");
            display_suit_pressure_total_eva2.text = "Suit pressure total: " + telemetryEva2.suit_pressure_total.ToString("F2");
            display_fan_pri_rpm_eva2.text = "Fan primary RPM: " + telemetryEva2.fan_pri_rpm.ToString("F0");
            display_fan_sec_rpm_eva2.text = "Fan secondary RPM: " + telemetryEva2.fan_sec_rpm.ToString("F0");
            display_helmet_pressure_co2_eva2.text = "Helmet pressure CO2: " + telemetryEva2.helmet_pressure_co2.ToString("F2");
            display_scrubber_a_co2_storage_eva2.text = "Scrubber A CO2: " + telemetryEva2.scrubber_a_co2_storage.ToString("F1"); 
            display_scrubber_b_co2_storage_eva2.text = "Scrubber B CO2: " + telemetryEva2.scrubber_b_co2_storage.ToString("F1");
            display_temperature_eva2.text = "Temperature: " + telemetryEva2.temperature.ToString("F1");
            display_coolant_ml_eva2.text = "Coolant ML: " + telemetryEva2.coolant_ml.ToString("F0");
            display_coolant_gas_pressure_eva2.text = "Coolant gas pressure: " + telemetryEva2.coolant_gas_pressure.ToString("F1");
            display_coolant_liquid_pressure_eva2.text = "Coolant liquid pressure: " + telemetryEva2.coolant_liquid_pressure.ToString("F1");
        }
        else
        {
            Debug.LogWarning("VitalsDisplay: EVA2 telemetry data is null");
            SetEVA2DisplaysToDefault("No EVA2 Data");
        }
    }

    private void SetAllDisplaysToDefault(string message)
    {
        SetEVA1DisplaysToDefault(message);
        SetEVA2DisplaysToDefault(message);
    }

    private void SetEVA1DisplaysToDefault(string message)
    {
        if (display_batt_time_left_eva1) display_batt_time_left_eva1.text = message;
        if (display_oxy_pri_storage_eva1) display_oxy_pri_storage_eva1.text = message;
        if (display_oxy_sec_storage_eva1) display_oxy_sec_storage_eva1.text = message;
        if (display_oxy_time_left_eva1) display_oxy_time_left_eva1.text = message;
        if (display_heart_rate_eva1) display_heart_rate_eva1.text = message;
        if (display_oxy_consumption_eva1) display_oxy_consumption_eva1.text = message;
        if (display_co2_production_eva1) display_co2_production_eva1.text = message;
        if (display_suit_pressure_oxy_eva1) display_suit_pressure_oxy_eva1.text = message;
        if (display_suit_pressure_co2_eva1) display_suit_pressure_co2_eva1.text = message;
        if (display_suit_pressure_other_eva1) display_suit_pressure_other_eva1.text = message;
        if (display_suit_pressure_total_eva1) display_suit_pressure_total_eva1.text = message;
        if (display_fan_pri_rpm_eva1) display_fan_pri_rpm_eva1.text = message;
        if (display_fan_sec_rpm_eva1) display_fan_sec_rpm_eva1.text = message;
        if (display_helmet_pressure_co2_eva1) display_helmet_pressure_co2_eva1.text = message;
        if (display_scrubber_a_co2_storage_eva1) display_scrubber_a_co2_storage_eva1.text = message;
        if (display_scrubber_b_co2_storage_eva1) display_scrubber_b_co2_storage_eva1.text = message;
        if (display_temperature_eva1) display_temperature_eva1.text = message;
        if (display_coolant_ml_eva1) display_coolant_ml_eva1.text = message;
        if (display_coolant_gas_pressure_eva1) display_coolant_gas_pressure_eva1.text = message;
        if (display_coolant_liquid_pressure_eva1) display_coolant_liquid_pressure_eva1.text = message;
    }

    private void SetEVA2DisplaysToDefault(string message)
    {
        if (display_batt_time_left_eva2) display_batt_time_left_eva2.text = message;
        if (display_oxy_pri_storage_eva2) display_oxy_pri_storage_eva2.text = message;
        if (display_oxy_sec_storage_eva2) display_oxy_sec_storage_eva2.text = message;
        if (display_oxy_time_left_eva2) display_oxy_time_left_eva2.text = message;
        if (display_heart_rate_eva2) display_heart_rate_eva2.text = message;
        if (display_oxy_consumption_eva2) display_oxy_consumption_eva2.text = message;
        if (display_co2_production_eva2) display_co2_production_eva2.text = message;
        if (display_suit_pressure_oxy_eva2) display_suit_pressure_oxy_eva2.text = message;
        if (display_suit_pressure_co2_eva2) display_suit_pressure_co2_eva2.text = message;
        if (display_suit_pressure_other_eva2) display_suit_pressure_other_eva2.text = message;
        if (display_suit_pressure_total_eva2) display_suit_pressure_total_eva2.text = message;
        if (display_fan_pri_rpm_eva2) display_fan_pri_rpm_eva2.text = message;
        if (display_fan_sec_rpm_eva2) display_fan_sec_rpm_eva2.text = message;
        if (display_helmet_pressure_co2_eva2) display_helmet_pressure_co2_eva2.text = message;
        if (display_scrubber_a_co2_storage_eva2) display_scrubber_a_co2_storage_eva2.text = message;
        if (display_scrubber_b_co2_storage_eva2) display_scrubber_b_co2_storage_eva2.text = message;
        if (display_temperature_eva2) display_temperature_eva2.text = message;
        if (display_coolant_ml_eva2) display_coolant_ml_eva2.text = message;
        if (display_coolant_gas_pressure_eva2) display_coolant_gas_pressure_eva2.text = message;
        if (display_coolant_liquid_pressure_eva2) display_coolant_liquid_pressure_eva2.text = message;
    }
}

// Removed VitalsResponse, EvaData, and Telemetry classes as they are now defined in BackendDataService.cs
// or covered by AllDataResponse and its nested classes.