using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Ingress : MonoBehaviour
{
    // Game objects representing UIA and DCU controls
    public GameObject EMU1_POWER;
    public GameObject EV1_SUPPLY;
    public GameObject EV1_WASTE;
    public GameObject EV2_SUPPLY;
    public GameObject EV2_WASTE;
    public GameObject EMU2_POWER;
    public GameObject EMU1_OXY;
    public GameObject EMU2_OXY;
    public GameObject O2_VENT;
    public GameObject DEPRESS_PUMP;

    public TSS_DATA tss_obj;
    public TMP_Text text;

    Dictionary<int, Dictionary<int, bool>> steps = new Dictionary<int, Dictionary<int, bool>>();
    Dictionary<int, bool> step1 = new Dictionary<int, bool>();
    Dictionary<int, bool> step2 = new Dictionary<int, bool>();
    Dictionary<int, bool> step3 = new Dictionary<int, bool>();
    Dictionary<int, bool> step4 = new Dictionary<int, bool>();

    void Start()
    {
        tss_obj.duringEVA = false;
        InitializeSteps();
    }

    void Update()
    {
        ExecuteSteps();
    }

    void InitializeSteps()
    {
        EMU1_POWER.SetActive(false);
        EV1_SUPPLY.SetActive(false);
        EV1_WASTE.SetActive(false);
        EV2_SUPPLY.SetActive(false);
        EV2_WASTE.SetActive(false);
        EMU2_POWER.SetActive(false);
        EMU1_OXY.SetActive(false);
        EMU2_OXY.SetActive(false);
        O2_VENT.SetActive(false);
        DEPRESS_PUMP.SetActive(false);

        // Step 1
        for (int i = 1; i <= 3; i++)
        {
            step1.Add(i, false);
        }
        steps.Add(1, step1);

        // Step 2
        for (int i = 1; i <= 3; i++)
        {
            step2.Add(i, false);
        }
        steps.Add(2, step2);

        // Step 3
        for (int i = 1; i <= 4; i++)
        {
            step3.Add(i, false);
        }
        steps.Add(3, step3);

        // Step 4
        for (int i = 1; i <= 2; i++)
        {
            step4.Add(i, false);
        }
        steps.Add(4, step4);
    }

    void ExecuteSteps()
    {
        // Step 1: Connect UIA to DCU and start Depress
        if (!steps[1].All(x => x.Value == true))
        {
            // UIA and DCU 1. EV1 and EV2 connect UIA and DCU umbilical
            if (!steps[1][1])
            {
                text.text = "Connect EV-1 UIA and DCU umbilical.";
                EMU1_POWER.SetActive(!tss_obj.uia.uia.eva1_power);
                if (!tss_obj.uia.uia.eva1_power) return;
                if (!dcu_batt_msg(true)) return;
                steps[1][1] = true;
            }

            // UIA 2. EV-1, EV-2 EMU PWR – ON
            if (!steps[1][2])
            {
                // Activate EMU power
                // *done in above step.* 
                steps[1][2] = true;
            }

            // BOTH DCU 3. BATT – UMB
            if (!steps[1][3])
            {
                steps[1][3] = true;
            }
        }

        // Step 2: Vent O2 Tanks
        if (!steps[2].All(x => x.Value == true))
        {
            // UIA 1. OXYGEN O2 VENT – OPEN
            if (!steps[2][1])
            {
                text.text = "Open O2 vent.";
                O2_VENT.SetActive(!tss_obj.uia.uia.oxy_vent);
                if (!tss_obj.uia.uia.oxy_vent) return;
                steps[2][1] = true;
            }

            // HMD 2. Wait until both Primary and Secondary OXY tanks are < 10psi
            if (!steps[2][2])
            {
                double p1 = tss_obj.tel.telemetry.eva1.oxy_pri_storage;
                double s1 = tss_obj.tel.telemetry.eva1.oxy_sec_storage;
                text.text = "eva1 primary: " + p1 + " secondary: " + s1;//+ " eva2 primary: " + p2 + " secondary: " + s2 + ".";
                bool empty = p1 < 10 && s1 < 10; //&& p2 == 0 && s2 == 0;
                if (!empty) return;

                steps[2][2] = true;
            }

            // UIA 3. OXYGEN O2 VENT – CLOSE
            if (!steps[2][3])
            {
                // Close O2 vent
                text.text = "Close O2 vent.";
                O2_VENT.SetActive(tss_obj.uia.uia.oxy_vent);
                if (tss_obj.uia.uia.oxy_vent) return;
                steps[2][3] = true;
            }
        }

        // Step 3: Empty Water Tanks
        if (!steps[3].All(x => x.Value == true))
        {
            // BOTH DCU 1. PUMP – OPEN
            if (!steps[3][1])
            {
                // Open pump
                if (!dcu_pump_msg(true)) return;
                steps[3][1] = true;
            }

            // UIA 2. EV-1, EV-2 WASTE WATER – OPEN
            if (!steps[3][2])
            {
                // Open waste water valve
                text.text = "Open EV-1 waste water.";
                EV1_WASTE.SetActive(!tss_obj.uia.uia.eva1_water_waste);
                if (!tss_obj.uia.uia.eva1_water_waste) return;
                steps[3][2] = true;
            }

            // HMD 3. Wait until water EV1 and EV2 Coolant tank is < 5%
            if (!steps[3][3])
            {
                double p1 = tss_obj.tel.telemetry.eva1.coolant_ml;
                text.text = "Coolant: " + p1 + ".";
                if (p1 < 5) return;

                steps[3][3] = true;
            }

            // UIA 4. EV-1, EV-2 WASTE WATER – CLOSE
            if (!steps[3][4])
            {
                // Close waste water valve
                text.text = "Close EV-1 waste water.";
                EV1_WASTE.SetActive(tss_obj.uia.uia.eva1_water_waste);
                if (tss_obj.uia.uia.eva1_water_waste) return;
                steps[3][4] = true;
            }
        }

        // Step 4: Disconnect UIA from DCU
        if (!steps[4].All(x => x.Value == true))
        {
            // UIA 1. EV-1, EV-2 EMU PWR – OFF
            if (!steps[4][1])
            {
                // Deactivate EMU power
                text.text = "Toggle EV-1 EMU PWR off.";
                EMU1_POWER.SetActive(tss_obj.uia.uia.eva1_power);
                if (tss_obj.uia.uia.eva1_power) return;

                steps[4][1] = true;
            }

            // DCU 2. EV1 and EV2 disconnect umbilical
            if (!steps[4][2])
            {
                if (!dcu_batt_msg(false)) return;
                steps[4][2] = true;
                text.text = "Ingress Complete!";
            }
        }
    }

    bool dcu_batt_msg(bool mode)
    {
        string modeText = mode ? "umbilical" : "local";
        if (tss_obj.dcu.dcu.eva1.batt != mode)
        {
            text.text = "Switch DCU batt to " + modeText + ". ";
            return false;
        }
        return true;
    }
    bool dcu_oxy_msg(bool mode)
    {
        string modeText = mode ? "primary" : "secondary";
        if (tss_obj.dcu.dcu.eva1.oxy != mode)
        {
            text.text = "Switch DCU oxy to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_comm_msg(bool mode)
    {
        string modeText = mode ? "A" : "B";
        if (tss_obj.dcu.dcu.eva1.comm != mode)
        {
            text.text = "Switch DCU comm to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_fan_msg(bool mode)
    {
        string modeText = mode ? "primary" : "secondary";
        if (tss_obj.dcu.dcu.eva1.fan != mode)
        {
            text.text = "Switch DCU fan to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_pump_msg(bool mode)
    {
        string modeText = mode ? "open" : "closed";
        if (tss_obj.dcu.dcu.eva1.pump != mode)
        {
            text.text = "Switch DCU pump to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_co2_msg(bool mode)
    {
        string modeText = mode ? "A" : "B";
        if (tss_obj.dcu.dcu.eva1.co2 != mode)
        {
            text.text = "Switch DCU co2 to " + modeText + ". ";
            return false;
        }
        return true;
    }

}