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

public class TSS_DATA : MonoBehaviour
{
    public class UIAData
    {
        public UIA uia { get; set; }
        public class UIA
        {
            public bool eva1_power { get; set; }
            public bool eva1_oxy { get; set; }
            public bool eva1_water_supply { get; set; }
            public bool eva1_water_waste { get; set; }
            public bool eva2_power { get; set; }
            public bool eva2_oxy { get; set; }
            public bool eva2_water_supply { get; set; }
            public bool eva2_water_waste { get; set; }
            public bool oxy_vent { get; set; }
            public bool depress { get; set; }
        }
    }

    public class TelemetryData
    {
        public Telemetry telemetry { get; set; }

        public class Telemetry
        {
            public int eva_time { get; set; }
            public EvaData eva1 { get; set; }
            public EvaData eva2 { get; set; }
            public class EvaData
            {
                public double batt_time_left { get; set; }
                public double oxy_pri_storage { get; set; }
                public double oxy_sec_storage { get; set; }
                public double oxy_pri_pressure { get; set; }
                public double oxy_sec_pressure { get; set; }
                public int oxy_time_left { get; set; }
                public double heart_rate { get; set; }
                public double oxy_consumption { get; set; }
                public double co2_production { get; set; }
                public double suit_pressure_oxy { get; set; }
                public double suit_pressure_co2 { get; set; }
                public double suit_pressure_other { get; set; }
                public double suit_pressure_total { get; set; }
                public double fan_pri_rpm { get; set; }
                public double fan_sec_rpm { get; set; }
                public double helmet_pressure_co2 { get; set; }
                public double scrubber_a_co2_storage { get; set; }
                public double scrubber_b_co2_storage { get; set; }
                public double temperature { get; set; }
                public double coolant_ml { get; set; }
                public double coolant_gas_pressure { get; set; }
                public double coolant_liquid_pressure { get; set; }
            }

        }

    }

    // Represents the "telemetry" object

    // Represents an EVA session ("eva1", "eva2", etc.)

    public class SpecData
    {
        public Spec spec { get; set; }
        public class Spec
        {
            public EvaData eva1 { get; set; }
            public EvaData eva2 { get; set; }
            public class EvaData
            {
                public string name { get; set; }
                public int id { get; set; }
                public ChemicalData data { get; set; }
                public class ChemicalData
                {
                    public double SiO2 { get; set; }
                    public double TiO2 { get; set; }
                    public double Al2O3 { get; set; }
                    public double FeO { get; set; }
                    public double MnO { get; set; }
                    public double MgO { get; set; }
                    public double CaO { get; set; }
                    public double K2O { get; set; }
                    public double P2O3 { get; set; }
                }

            }

        }

    }

    public class RoverData
    {
        public Rover rover { get; set; }
        public class Rover
        {
            public double posx { get; set; }
            public double posy { get; set; }
            public int qr_id { get; set; }
        }
    }

    public class IMUData
    {
        public DCU dcu { get; set; }
        public class DCU
        {
            public EvaData eva1 { get; set; }
            public EvaData eva2 { get; set; }
            public class EvaData
            {
                public double posx { get; set; }
                public double posy { get; set; }
                public double heading { get; set; }
            }
        }
    }

    public class DCUData
    {
        public DCU dcu { get; set; }
        public class DCU
        {
            public EvaData eva1 { get; set; }
            public EvaData eva2 { get; set; }
            public class EvaData
            {
                public bool batt { get; set; }
                public bool oxy { get; set; }
                public bool comm { get; set; }
                public bool fan { get; set; }
                public bool pump { get; set; }
                public bool co2 { get; set; }
            }
        }
    }

    public class COMMData
    {
        public COMM comm { get; set; }
        public class COMM
        {
            public bool comm_tower { get; set; }
        }
    }


    // TSSc Connection
    public TSScConnection TSSc;


    // UI Display
    public UIAData uia;
    public DCUData dcu;
    public RoverData rov;
    public SpecData spec;
    public TelemetryData tel;
    public COMMData com;
    public IMUData imu;
    public bool duringEVA = false;
    //this bool is so error checking only checks at appropriate times

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Check if the UIA data has been updated
        if (TSSc.isUIAUpdated())
        {
            Debug.Log("UIA Updated");
            string UIAJsonString = TSSc.GetUIAJsonString();
            uia = JsonConvert.DeserializeObject<UIAData>(UIAJsonString);
            
            //        UIAJsonDisplay.text = uiaData.uia.eva1_power.ToString();
            // Use the deserialized data as needed
            // UIAJsonDisplay.text = eva1Power.ToString();
        }

        // Check if the DCU data has been updated
        if (TSSc.isDCUUpdated())
        {
            Debug.Log("DCU Updated");

            // Get the Updated DCU Json
            string DCUJsonString = TSSc.GetDCUJsonString();
            dcu = JsonConvert.DeserializeObject<DCUData>(DCUJsonString);
            //display the dcu data as a nicely formatted string


            //DCUJsonDisplay.text = formattedText;
            // Displays the DCU data to the screen (Your's should be more complicated)
            // DCUJsonDisplay.text = DCUJsonString;
        }

        // Check if the ROVER data has been updated
        if (TSSc.isROVERUpdated())
        {
            Debug.Log("ROVER Updated");

            // Get the Updated ROVER Json
            string ROVERJsonString = TSSc.GetROVERJsonString();

            rov = JsonConvert.DeserializeObject<RoverData>(ROVERJsonString);

            // Displays the ROVER data to the screen (Your's should be more complicated)
            // ROVERJsonDisplay.text = ROVERJsonString;
        }

        // Check if the SPEC data has been updated
        if (TSSc.isSPECUpdated())
        {
            Debug.Log("SPEC Updated");
            // Get the Updated SPEC Json
            string SPECJsonString = TSSc.GetSPECJsonString();

            //Display the spec data as a nicely formatted string
            spec = JsonConvert.DeserializeObject<SpecData>(SPECJsonString);

            //            SPECJsonDisplay.text = formattedText;
            // SPECJsonDisplay.text = "SPEC Data: \n" + eva1Data + "\n\n" + eva2Data;

            // Displays the SPEC data to the screen (Your's should be more complicated)
            // SPECJsonDisplay.text = SPECJsonString;
        }

        // Check if the TELEMETRY data has been updated
        if (TSSc.isTELEMETRYUpdated())
        {
            Debug.Log("TELEMETRY Updated");

            // Get the Updated TELEMETRY Json
            string TELEMETRYJsonString = TSSc.GetTELEMETRYJsonString();

            tel = JsonConvert.DeserializeObject<TelemetryData>(TELEMETRYJsonString);
            //TELEMETRYJsonDisplay.ext = telemetryData.ToString();
            // Displays the TELEMETRY data to the screen (Your's should be more complicated)
            // TELEMETRYJsonDisplay.text = TELEMETRYJsonString;
        }

        // Check if the COMM data has been updated
        if (TSSc.isCOMMUpdated())
        {
            Debug.Log("COMM Updated");

            // Get the Updated COMM Json
            string COMMJsonString = TSSc.GetCOMMJsonString();
            com = JsonConvert.DeserializeObject<COMMData>(COMMJsonString);
            //bool commTower = commData.comm.comm_tower;
            //COMMJsonDisplay.text = commData.ToString();

            // Displays the COMM data to the screen (Your's should be more complicated)
            // COMMJsonDisplay.text = COMMJsonString;
        }

        // don't know why this isn't displaying properly --\_(-_-)_/--
        if (TSSc.isIMUUpdated())
        {
            Debug.Log("IMU Updated");

            // Get the Updated IMU Json
            string IMUJsonString = TSSc.GetIMUJsonString();
            imu = JsonConvert.DeserializeObject<IMUData>(IMUJsonString);
            //IMUJsonDisplay.text = imuData.ToString();
            // Displays the IMU data to the screen (Your's should be more complicated)
            // IMUJsonDisplay.text = IMUJsonString;
        }
    }
}
