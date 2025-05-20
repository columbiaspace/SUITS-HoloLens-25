using System.Collections;
// using System.Collections.Generic; // No longer needed for local data structures
using UnityEngine;
using TMPro;
// using UnityEngine.Networking; // No longer needed for direct API calls

public class vitals2025 : MonoBehaviour // Changed class name to follow C# conventions (PascalCase)
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

    void Start()
    {
        // Initial text can be set here or just wait for the first data update
        display_oxy_sec_storage_eva1.text = "Waiting for data...";
    }

    void Update()
    {
        if (BackendDataService.Instance != null && BackendDataService.Instance.LatestData != null)
        {
            AllDataResponse data = BackendDataService.Instance.LatestData;

            if (data.eva1 != null && data.eva1.telemetry != null)
            {
                var telemetryEva1 = data.eva1.telemetry;
                display_batt_time_left_eva1.text = "Battery time left: " + telemetryEva1.batt_time_left.ToString("F1");
                display_oxy_pri_storage_eva1.text = "Primary oxygen storage: " + telemetryEva1.oxy_pri_storage.ToString("F1");
                display_oxy_sec_storage_eva1.text = "Secondary oxygen storage: " + telemetryEva1.oxy_sec_storage.ToString("F1");
                display_oxy_time_left_eva1.text = "Oxygen time left: " + telemetryEva1.oxy_time_left.ToString();
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
                // Optionally clear EVA1 fields or show "No Data"
            }

            if (data.eva2 != null && data.eva2.telemetry != null)
            {
                var telemetryEva2 = data.eva2.telemetry;
                display_batt_time_left_eva2.text = "Battery time left: " + telemetryEva2.batt_time_left.ToString("F1");
                display_oxy_pri_storage_eva2.text = "Primary oxygen storage: " + telemetryEva2.oxy_pri_storage.ToString("F1");
                display_oxy_sec_storage_eva2.text = "Secondary oxygen storage: " + telemetryEva2.oxy_sec_storage.ToString("F1");
                display_oxy_time_left_eva2.text = "Oxygen time left: " + telemetryEva2.oxy_time_left.ToString();
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
                // Optionally clear EVA2 fields or show "No Data"
            }
        }
        else
        {
            // Handle case where BackendDataService or its data isn't ready yet
            // For example, set text to "Connecting..." or "Waiting for data..."
            // This is partly handled by the initial Start() text too.
        }
    }
}

// Removed VitalsResponse, EvaData, and Telemetry classes as they are now defined in BackendDataService.cs
// or covered by AllDataResponse and its nested classes.