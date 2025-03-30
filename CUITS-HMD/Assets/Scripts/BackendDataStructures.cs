using System.Collections.Generic;
using Newtonsoft.Json; // Make sure Newtonsoft.Json is added to your Unity project

// Note: Field names match the JSON keys from the FastAPI backend's /all endpoint.
// Newtonsoft.Json's [JsonProperty] attribute can be used if C# naming conventions differ significantly,
// but it seems unnecessary here as the backend uses snake_case which works fine in C#.

[System.Serializable]
public class EvaDcuData
{
    public float battery { get; set; }
    public float oxygen { get; set; }
    public float comms { get; set; }
    public float fan { get; set; }
    public float pump { get; set; }
    public float co2 { get; set; }
}

[System.Serializable]
public class EvaImuData
{
    public float posx { get; set; }
    public float posy { get; set; }
    public float heading { get; set; }
}

[System.Serializable]
public class EvaTelemetryData // NEW: Structure for detailed telemetry
{
    public float batt_time_left { get; set; }
    public float oxy_pri_storage { get; set; }
    public float oxy_sec_storage { get; set; }
    public float oxy_pri_pressure { get; set; }
    public float oxy_sec_pressure { get; set; }
    public float oxy_time_left { get; set; }
    public float heart_rate { get; set; }
    public float oxy_consumption { get; set; }
    public float co2_production { get; set; }
    public float suit_pressure_oxy { get; set; }
    public float suit_pressure_co2 { get; set; }
    public float suit_pressure_other { get; set; }
    public float suit_pressure_total { get; set; }
    public float fan_pri_rpm { get; set; }
    public float fan_sec_rpm { get; set; }
    public float helmet_pressure_co2 { get; set; }
    public float scrubber_a_co2_storage { get; set; }
    public float scrubber_b_co2_storage { get; set; }
    public float temperature { get; set; }
    public float coolant_ml { get; set; }
    public float coolant_gas_pressure { get; set; }
    public float coolant_liquid_pressure { get; set; }
}

[System.Serializable]
public class EvaDataContainer
{
    public EvaDcuData dcu { get; set; }
    public EvaImuData imu { get; set; }
    public EvaTelemetryData telemetry { get; set; } // NEW: Added telemetry data
}

[System.Serializable]
public class ErrorData
{
    public float error1 { get; set; }
    public float error2 { get; set; }
    public float error3 { get; set; }
}

// RoverData Removed as it's not sent by backend anymore
/*
[System.Serializable]
public class RoverData
{
    public float posx { get; set; }
    public float posy { get; set; }
    public float qr_id { get; set; }
}
*/

[System.Serializable]
public class UiaData
{
    public float emu1_power { get; set; }
    public float ev1_supply { get; set; }
    public float ev1_waste { get; set; }
    public float ev1_oxygen { get; set; }
    public float emu2_power { get; set; }
    public float ev2_supply { get; set; }
    public float ev2_waste { get; set; }
    public float ev2_oxygen { get; set; }
    public float o2_vent { get; set; }
    public float depress_pump { get; set; }
}

[System.Serializable]
public class AllTssData
{
    public float eva_time { get; set; } // NEW: Added eva_time
    public EvaDataContainer eva1 { get; set; }
    public EvaDataContainer eva2 { get; set; }
    public ErrorData errors { get; set; }
    // public RoverData rover { get; set; } // REMOVED
    public UiaData uia { get; set; }
    // public List<float> lidar { get; set; } // REMOVED
}
