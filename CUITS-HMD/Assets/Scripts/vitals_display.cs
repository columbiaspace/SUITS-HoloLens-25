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
    public TSS_DATA TSS;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        display_batt_time_left_eva1.text = "Battery time left: {" + TSS.tel.telemetry.eva1.batt_time_left.ToString() + "}";
        display_oxy_pri_storage_eva1.text = "Primary oxygen storage: {" + TSS.tel.telemetry.eva1.oxy_pri_storage.ToString() + "}";
        display_oxy_sec_storage_eva1.text = "Secondary oxygen storage: {" + TSS.tel.telemetry.eva1.oxy_sec_storage.ToString() + "}";
        display_oxy_time_left_eva1.text = "Oxygen time left: {" + TSS.tel.telemetry.eva1.oxy_time_left.ToString() + "}";
        display_heart_rate_eva1.text = "Heart rate: {" + TSS.tel.telemetry.eva1.heart_rate.ToString() + "}";
        display_oxy_consumption_eva1.text = "Oxygen consumption: {" + TSS.tel.telemetry.eva1.oxy_consumption.ToString() + "}";
        display_co2_production_eva1.text = "CO2 production: {" + TSS.tel.telemetry.eva1.co2_production.ToString() + "}";
        display_suit_pressure_oxy_eva1.text = "Suit oxygen pressure: {" + TSS.tel.telemetry.eva1.suit_pressure_oxy.ToString() + "}";
        display_suit_pressure_co2_eva1.text = "Suit CO2 pressure: {" + TSS.tel.telemetry.eva1.suit_pressure_co2.ToString() + "}";
        display_suit_pressure_other_eva1.text = "Suit other pressure: {" + TSS.tel.telemetry.eva1.suit_pressure_other.ToString() + "}";
        display_suit_pressure_total_eva1.text = "Suit total pressure: {" + TSS.tel.telemetry.eva1.suit_pressure_total.ToString() + "}";
        display_fan_pri_rpm_eva1.text = "Primary fan rpm: {" + TSS.tel.telemetry.eva1.fan_pri_rpm.ToString() + "}";
        display_fan_sec_rpm_eva1.text = "Secondary fan rpm: {" + TSS.tel.telemetry.eva1.fan_sec_rpm.ToString() + "}";
        display_helmet_pressure_co2_eva1.text = "Helmet CO2 pressure: {" + TSS.tel.telemetry.eva1.helmet_pressure_co2.ToString() + "}";
        display_scrubber_a_co2_storage_eva1.text = "Scrubber A CO2 storage: {" + TSS.tel.telemetry.eva1.scrubber_a_co2_storage.ToString() + "}";
        display_scrubber_b_co2_storage_eva1.text = "Scrubber B CO2 storage: {" + TSS.tel.telemetry.eva1.scrubber_b_co2_storage.ToString() + "}";
        display_temperature_eva1.text = "Temperature: {" + TSS.tel.telemetry.eva1.temperature.ToString() + "}";
        display_coolant_ml_eva1.text = "Coolant ml: {" + TSS.tel.telemetry.eva1.coolant_ml.ToString() + "}";
        display_coolant_gas_pressure_eva1.text = "Coolant gas pressure: {" + TSS.tel.telemetry.eva1.coolant_gas_pressure.ToString() + "}";
        display_coolant_liquid_pressure_eva1.text = "Coolant liquid pressure: {" + TSS.tel.telemetry.eva1.coolant_liquid_pressure.ToString() + "}";

        display_batt_time_left_eva2.text = "Battery time left: {" + TSS.tel.telemetry.eva2.batt_time_left.ToString() + "}";
        display_oxy_pri_storage_eva2.text = "Primary oxygen storage: {" + TSS.tel.telemetry.eva2.oxy_pri_storage.ToString() + "}";
        display_oxy_sec_storage_eva2.text = "Secondary oxygen storage: {" + TSS.tel.telemetry.eva2.oxy_sec_storage.ToString() + "}";
        display_oxy_time_left_eva2.text = "Oxygen time left: {" + TSS.tel.telemetry.eva2.oxy_time_left.ToString() + "}";
        display_heart_rate_eva2.text = "Heart rate: {" + TSS.tel.telemetry.eva2.heart_rate.ToString() + "}";
        display_oxy_consumption_eva2.text = "Oxygen consumption: {" + TSS.tel.telemetry.eva2.oxy_consumption.ToString() + "}";
        display_co2_production_eva2.text = "CO2 production: {" + TSS.tel.telemetry.eva2.co2_production.ToString() + "}";
        display_suit_pressure_oxy_eva2.text = "Suit oxygen pressure: {" + TSS.tel.telemetry.eva2.suit_pressure_oxy.ToString() + "}";
        display_suit_pressure_co2_eva2.text = "Suit CO2 pressure: {" + TSS.tel.telemetry.eva2.suit_pressure_co2.ToString() + "}";
        display_suit_pressure_other_eva2.text = "Suit other pressure: {" + TSS.tel.telemetry.eva2.suit_pressure_other.ToString() + "}";
        display_suit_pressure_total_eva2.text = "Suit total pressure: {" + TSS.tel.telemetry.eva2.suit_pressure_total.ToString() + "}";
        display_fan_pri_rpm_eva2.text = "Primary fan rpm: {" + TSS.tel.telemetry.eva2.fan_pri_rpm.ToString() + "}";
        display_fan_sec_rpm_eva2.text = "Secondary fan rpm: {" + TSS.tel.telemetry.eva2.fan_sec_rpm.ToString() + "}";
        display_helmet_pressure_co2_eva2.text = "Helmet CO2 pressure: {" + TSS.tel.telemetry.eva2.helmet_pressure_co2.ToString() + "}";
        display_scrubber_a_co2_storage_eva2.text = "Scrubber A CO2 storage: {" + TSS.tel.telemetry.eva2.scrubber_a_co2_storage.ToString() + "}";
        display_scrubber_b_co2_storage_eva2.text = "Scrubber B CO2 storage: {" + TSS.tel.telemetry.eva2.scrubber_b_co2_storage.ToString() + "}";
        display_temperature_eva2.text = "Temperature: {" + TSS.tel.telemetry.eva2.temperature.ToString() + "}";
        display_coolant_ml_eva2.text = "Coolant ml: {" + TSS.tel.telemetry.eva2.coolant_ml.ToString() + "}";
        display_coolant_gas_pressure_eva2.text = "Coolant gas pressure: {" + TSS.tel.telemetry.eva2.coolant_gas_pressure.ToString() + "}";
        display_coolant_liquid_pressure_eva2.text = "Coolant liquid pressure: {" + TSS.tel.telemetry.eva2.coolant_liquid_pressure.ToString() + "}";
    }

    public void eva1(){
        title.text = "EVA1 Telemetry";
    }

    public void eva2(){
        title.text = "EVA2 Telemetry";
    }
}
