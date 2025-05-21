using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; // Using Newtonsoft.Json for more robust deserialization
using System; // For System.Serializable

// --- Data structures for /all endpoint --- 

[System.Serializable]
public class BackendProcedureStep
{
    public string procedure_name; // e.g., "EVA Egress", "EVA Ingress"
    public int step_number;
    public string instruction_text;
    public List<ProcedureActionableItem> actionable_items; 
    public bool is_completed; 
}

[System.Serializable]
public class ProcedureActionableItem
{
    public string item_name; 
    public string target_state_description; 
    public bool current_state_matches; 
}

[System.Serializable]
public class PinData // Structure for individual pins
{
    // From main.py Pin class: ev, posx, posy. Add label if customizable.
    public int ev; 
    public float posx;
    public float posy;
    public string label; // Assuming pins can have labels
}

[System.Serializable]
public class WarningData // Structure for individual warnings
{
    public string id; // Unique ID for the warning
    public string message; // Warning text
    public int severity; // e.g., 1=low, 2=medium, 3=high
    public string source; // e.g., "SYSTEM_TELEMETRY", "ROVER_MANUAL"
}

[System.Serializable]
public class AllDataResponse
{
    public float eva_time;
    public EvaStates eva_states;
    public EvaData eva1;
    public EvaData eva2;
    public Errors errors;
    public UIAData uia;
    public RoverData rover;
    public double last_updated; // Changed from long to double for Unix timestamp in JSON

    // New fields for integrated data
    public BackendProcedureStep current_procedure_step; 
    public List<PinData> pins; 
    public List<WarningData> warnings;
}

[System.Serializable]
public class EvaStates
{
    public double started;
    public double paused;
    public double completed;
    public double total_time;
    public double uia_started;
    public double uia_completed;
    public double uia_time;
    public double dcu_started;
    public double dcu_completed;
    public double dcu_time;
    public double rover_started;
    public double rover_completed;
    public double rover_time;
    public double spec_started;
    public double spec_completed;
    public double spec_time;
}

[System.Serializable]
public class EvaData
{
    public DCUData dcu;
    public IMUData imu;
    public TelemetryData telemetry;
    public SpecData spec;
}

[System.Serializable]
public class DCUData
{
    // Field names from tss_client.py: battery, oxygen, comms, fan, pump, co2
    // All are float values (0.0 or 1.0) representing switch states
    public float battery;
    public float oxygen;
    public float comms;
    public float fan;
    public float pump;
    public float co2;
}

[System.Serializable]
public class IMUData
{
    public float posx;
    public float posy;
    public float heading;
}

[System.Serializable]
public class TelemetryData // Corresponds to _get_eva_telemetry in tss_client.py
{
    public double batt_time_left;
    public double oxy_pri_storage;
    public double oxy_sec_storage;
    public double oxy_pri_pressure;
    public double oxy_sec_pressure;
    public double oxy_time_left; // Changed from int to double
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

[System.Serializable]
public class SpecData
{
    public float id; // Changed from int to float to match the 0.0 in JSON
    public string name; // Rock name added by backend
    public double SiO2;
    public double TiO2;
    public double Al2O3;
    public double FeO;
    public double MnO;
    public double MgO;
    public double CaO;
    public double K2O;
    public double P2O3;
    public double other;
}

[System.Serializable]
public class Errors // from tss_client.get_error_states()
{
    public float error1;
    public float error2;
    public float error3;
}

[System.Serializable]
public class UIAData // from tss_client.get_uia()
{
    // Field names from tss_client.py get_uia() and main.py (snake_case)
    // All are float (0.0/1.0) representing switch states
    public float emu1_power;
    public float ev1_supply;
    public float ev1_waste;
    public float ev1_oxygen;
    public float emu2_power;
    public float ev2_supply;
    public float ev2_waste;
    public float ev2_oxygen;
    public float o2_vent;
    public float depress_pump;
}

[System.Serializable]
public class RoverData // from tss_client.get_rover()
{
    public float posx;
    public float posy;
    public float poi_1_x;
    public float poi_1_y;
    public float poi_2_x;
    public float poi_2_y;
    public float poi_3_x;
    public float poi_3_y;
    public float ping;
}


public class BackendDataService : MonoBehaviour
{
    public static BackendDataService Instance { get; private set; }

    public AllDataResponse LatestData { get; private set; }

    // Configurable backend URL (base part, e.g., http://127.0.0.1:8000)
    [Header("Backend Configuration")]
    public string backendBaseUrl = "http://127.0.0.1:8000"; 
    private string allDataEndpoint = "/all";

    public float fetchInterval = 1.0f; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(FetchDataRoutine());
    }

    IEnumerator FetchDataRoutine()
    {
        while (true)
        {
            yield return FetchDataFromServer();
            yield return new WaitForSeconds(fetchInterval);
        }
    }

    public IEnumerator FetchDataFromServer()
    {
        string fullUrl = backendBaseUrl + allDataEndpoint;
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                try
                {
                    // Use settings to handle floating-point values correctly
                    var settings = new JsonSerializerSettings
                    {
                        FloatParseHandling = FloatParseHandling.Double
                    };
                    
                    LatestData = JsonConvert.DeserializeObject<AllDataResponse>(json, settings);
                    if (LatestData != null)
                    {
                         Debug.Log($"Successfully fetched and parsed data from {fullUrl}.");
                    }
                    else
                    {
                        Debug.LogError($"Failed to deserialize data from {fullUrl}. LatestData is null.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deserializing JSON from {fullUrl}: {e.Message}\nJSON: {json}");
                }
            }
            else
            {
                Debug.LogError($"Error fetching data from {fullUrl}: {request.error}");
            }
        }
    }

    public void RequestDataRefresh()
    {
        StartCoroutine(FetchDataFromServer());
    }
} 