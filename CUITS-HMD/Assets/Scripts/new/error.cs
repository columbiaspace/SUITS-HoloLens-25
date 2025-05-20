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
    public TSS_DATA TSS;
    public TMP_Text display;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(TSS.duringEVA == true)
        {
            // heart_rate
            if (TSS.tel.telemetry.eva1.heart_rate > 160)
            {
                display.text = "Detected heart rate too high: please slow down";
                return;
            }

            // suit_pressure_oxy
            if (TSS.tel.telemetry.eva1.suit_pressure_oxy < 3.5 || TSS.tel.telemetry.eva1.suit_pressure_oxy > 4.1)
            {
                display.text = "Swap to secondary oxygen tank";
                return;
            }

            // suit_pressure_co2
            if (TSS.tel.telemetry.eva1.suit_pressure_co2 > 0.1)
            {
                display.text = "Scrubber has filled up and must be vented, flip DCU CO2 switch";
                return;
            }

            // suit_pressure_other
            if (TSS.tel.telemetry.eva1.suit_pressure_other > 0.5)
            {
                display.text = "Partial pressure of all gases are not zero";
                return;
            }

            // suit_pressure_total
            if (TSS.tel.telemetry.eva1.suit_pressure_total < 3.5 || TSS.tel.telemetry.eva1.suit_pressure_total > 4.5)
            {
                // suit_pressure_oxy
                if (TSS.tel.telemetry.eva1.suit_pressure_oxy < 3.5 || TSS.tel.telemetry.eva1.suit_pressure_oxy > 4.1)
                {
                    display.text = "Swap to secondary oxygen tank";
                    return;
                }
                // scrubber_a_co2_storage and scrubber_b_co2_storage
                if (TSS.tel.telemetry.eva1.scrubber_a_co2_storage > 60 || TSS.tel.telemetry.eva1.scrubber_b_co2_storage > 60)
                {
                    display.text = "Vent collected carbon dioxide, flip DCU CO2 switch";
                    return;
                }
            }

            // helmet_pressure_co2
            if (TSS.tel.telemetry.eva1.helmet_pressure_co2 > 0.15)
            {
                display.text = "Swap to secondary fan";
                return;
            }

            // fan_pri_rpm and fan_sec_rpm
            if (TSS.tel.telemetry.eva1.fan_pri_rpm != 0)
            {
                if (TSS.tel.telemetry.eva1.fan_pri_rpm <= 20000)
                {
                    display.text = "Swap to secondary fan";
                    return;
                }
            }
            else if (TSS.tel.telemetry.eva1.fan_sec_rpm != 0)
            {
                if (TSS.tel.telemetry.eva1.fan_sec_rpm <= 20000)
                {
                    display.text = "Swap to primary fan";
                    return;
                }
            }

            // scrubber_a_co2_storage and scrubber_b_co2_storage
            if (TSS.tel.telemetry.eva1.scrubber_a_co2_storage > 60 || TSS.tel.telemetry.eva1.scrubber_b_co2_storage > 60)
            {
                display.text = "Vent collected carbon dioxide, flip DCU CO2 switch";
                return;
            }

            // temperature
            if (TSS.tel.telemetry.eva1.temperature > 90)
            {
                display.text = "Detected temperature too high: please slow down";
                return;
            }

            display.text = "";
        }
       

    }
}
