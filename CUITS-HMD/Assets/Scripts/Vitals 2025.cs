using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class vitals2025 : MonoBehaviour
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
        StartCoroutine(GetDataFromServer()); // Start the coroutine to fetch data from the server

    }

    IEnumerator GetDataFromServer()
    {
        while (true) {
            string url = "http://127.0.0.1:8000/all";
            UnityWebRequest request = UnityWebRequest.Get(url); // Create a GET request to the FastAPI endpoint
            yield return request.SendWebRequest(); // Tells Unity to wait for the request to complete

            if (request.result == UnityWebRequest.Result.Success) // Check if the request was successful
            {
                string json = request.downloadHandler.text; // Get the raw JSON string returned by FastAPI
                
                VitalsResponse data = JsonUtility.FromJson<VitalsResponse>(json); // Turns that JSON into a C# object
                Debug.Log(data.eva1); // Log the JSON response for debugging

                display_batt_time_left_eva1.text = "Battery time left: " + data.eva1.telemetry.batt_time_left.ToString();
                display_oxy_pri_storage_eva1.text = "Primary oxygen storage: " + data.eva1.telemetry.oxy_pri_storage.ToString();
                display_oxy_sec_storage_eva1.text = "Secondary oxygen storage: " + data.eva1.telemetry.oxy_sec_storage.ToString();
                display_oxy_time_left_eva1.text = "Oxygen time left: " + data.eva1.telemetry.oxy_time_left.ToString();
                display_heart_rate_eva1.text = "Heart rate: " + data.eva1.telemetry.heart_rate.ToString();
                display_oxy_consumption_eva1.text = "Oxygen consumption: " + data.eva1.telemetry.oxy_consumption.ToString();
                display_co2_production_eva1.text = "CO2 production: " + data.eva1.telemetry.co2_production.ToString();
                display_suit_pressure_oxy_eva1.text = "Suit pressure O2: " + data.eva1.telemetry.suit_pressure_oxy.ToString();
                display_suit_pressure_co2_eva1.text = "Suit pressure CO2: " + data.eva1.telemetry.suit_pressure_co2.ToString();
                display_suit_pressure_other_eva1.text = "Suit pressure other: " + data.eva1.telemetry.suit_pressure_other.ToString();
                display_suit_pressure_total_eva1.text = "Suit pressure total: " + data.eva1.telemetry.suit_pressure_total.ToString();
                display_fan_pri_rpm_eva1.text = "Fan primary RPM: " + data.eva1.telemetry.fan_pri_rpm.ToString();
                display_fan_sec_rpm_eva1.text = "Fan secondary RPM: " + data.eva1.telemetry.fan_sec_rpm.ToString();
                display_helmet_pressure_co2_eva1.text = "Helmet pressure CO2: " + data.eva1.telemetry.helmet_pressure_co2.ToString();
                display_scrubber_a_co2_storage_eva1.text = "Scrubber A CO2 storage: " + data.eva1.telemetry.scrubber_a_co2_storage.ToString();
                display_scrubber_b_co2_storage_eva1.text = "Scrubber B CO2 storage: " + data.eva1.telemetry.scrubber_b_co2_storage.ToString();
                display_temperature_eva1.text = "Temperature: " + data.eva1.telemetry.temperature.ToString();
                display_coolant_ml_eva1.text = "Coolant ML: " + data.eva1.telemetry.coolant_ml.ToString();
                display_coolant_gas_pressure_eva1.text = "Coolant gas pressure: " + data.eva1.telemetry.coolant_gas_pressure.ToString();
                display_coolant_liquid_pressure_eva1.text = "Coolant liquid pressure: " + data.eva1.telemetry.coolant_liquid_pressure.ToString();
                // Update the display for EVA2
                display_batt_time_left_eva2.text = "Battery time left: " + data.eva2.telemetry.batt_time_left.ToString();
                display_oxy_pri_storage_eva2.text = "Primary oxygen storage: " + data.eva2.telemetry.oxy_pri_storage.ToString();
                display_oxy_sec_storage_eva2.text = "Secondary oxygen storage: " + data.eva2.telemetry.oxy_sec_storage.ToString();
                display_oxy_time_left_eva2.text = "Oxygen time left: " + data.eva2.telemetry.oxy_time_left.ToString();
                display_heart_rate_eva2.text = "Heart rate: " + data.eva2.telemetry.heart_rate.ToString();
                display_oxy_consumption_eva2.text = "Oxygen consumption: " + data.eva2.telemetry.oxy_consumption.ToString();
                display_co2_production_eva2.text = "CO2 production: " + data.eva2.telemetry.co2_production.ToString();
                display_suit_pressure_oxy_eva2.text = "Suit pressure O2: " + data.eva2.telemetry.suit_pressure_oxy.ToString();
                display_suit_pressure_co2_eva2.text = "Suit pressure CO2: " + data.eva2.telemetry.suit_pressure_co2.ToString();
                display_suit_pressure_other_eva2.text = "Suit pressure other: " + data.eva2.telemetry.suit_pressure_other.ToString();
                display_suit_pressure_total_eva2.text = "Suit pressure total: " + data.eva2.telemetry.suit_pressure_total.ToString();
                display_fan_pri_rpm_eva2.text = "Fan primary RPM: " + data.eva2.telemetry.fan_pri_rpm.ToString();
                display_fan_sec_rpm_eva2.text = "Fan secondary RPM: " + data.eva2.telemetry.fan_sec_rpm.ToString();
                display_helmet_pressure_co2_eva2.text = "Helmet pressure CO2: " + data.eva2.telemetry.helmet_pressure_co2.ToString();
                display_scrubber_a_co2_storage_eva2.text = "Scrubber A CO2 storage: " + data.eva2.telemetry.scrubber_a_co2_storage.ToString(); 
                display_scrubber_b_co2_storage_eva2.text = "Scrubber B CO2 storage: " + data.eva2.telemetry.scrubber_b_co2_storage.ToString();
                display_temperature_eva2.text = "Temperature: " + data.eva2.telemetry.temperature.ToString();
                display_coolant_ml_eva2.text = "Coolant ML: " + data.eva2.telemetry.coolant_ml.ToString();
                display_coolant_gas_pressure_eva2.text = "Coolant gas pressure: " + data.eva2.telemetry.coolant_gas_pressure.ToString();
                display_coolant_liquid_pressure_eva2.text = "Coolant liquid pressure: " + data.eva2.telemetry.coolant_liquid_pressure.ToString();

            }
            else
            {
                Debug.LogError("API Error (Vitals): " + request.error); // If the request fails, log the error in console
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

}

// TimeResponse class is used to deserialize the JSON response from the FastAPI server
[System.Serializable] // Makes the class serializable so JsonUtility can turn JSON into this object
public class VitalsResponse
{
    // Fields should match the JSON keys returned by FastAPI
    public EvaData eva1;
    public EvaData eva2;
}

[System.Serializable] // Makes the class serializable so JsonUtility can turn JSON into this object
public class EvaData
{
    public Telemetry telemetry;
}

[System.Serializable] // Makes the class serializable so JsonUtility can turn JSON into this object
public class Telemetry
{
    public double batt_time_left;
    public double oxy_pri_storage;
    public double oxy_sec_storage;
    public double oxy_pri_pressure;
    public double oxy_sec_pressure;
    public int oxy_time_left;
    public double heart_rate;
    public double oxy_consumption;
    public double co2_production;
    public double suit_pressure_oxy;
    public double suit_pressure_co2;
    public double suit_pressure_other;
    public double suit_pressure_total;
    public double fan_pri_rpm;
    public double fan_sec_rpm;
    public double helmet_pressure_co2;
    public double scrubber_a_co2_storage;
    public double scrubber_b_co2_storage;
    public double temperature;
    public double coolant_ml;
    public double coolant_gas_pressure;
    public double coolant_liquid_pressure;
}