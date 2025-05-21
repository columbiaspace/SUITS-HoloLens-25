using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Egress : MonoBehaviour
{
    [Header("UI Indicators")]
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

    [Header("Text Display")]
    public TextMeshPro instructionText;
    
    // Track if EVA is active
    private bool duringEVA = false;

    // Non-serialized step tracking
    private Dictionary<int, Dictionary<int, bool>> steps;
    private Dictionary<int, bool> step1;
    private Dictionary<int, bool> step2;
    private Dictionary<int, bool> step3;
    private Dictionary<int, bool> step4;

    void Start()
    {
        InitializeSteps();
        HideAllIndicators();
    }

    private void InitializeSteps()
    {
        // Initialize all dictionaries
        steps = new Dictionary<int, Dictionary<int, bool>>();
        step1 = new Dictionary<int, bool>();
        step2 = new Dictionary<int, bool>();
        step3 = new Dictionary<int, bool>();
        step4 = new Dictionary<int, bool>();

        // Step 1
        for (int i = 1; i <= 4; i++)
        {
            step1.Add(i, false);
        }
        steps.Add(1, step1);

        // Step 2
        for (int i = 1; i <= 12; i++)
        {
            step2.Add(i, false);
        }
        steps.Add(2, step2);

        // Step 3
        for (int i = 1; i <= 8; i++)
        {
            step3.Add(i, false);
        }
        steps.Add(3, step3);

        // Step 4
        for (int i = 1; i <= 10; i++)
        {
            step4.Add(i, false);
        }
        steps.Add(4, step4);
    }

    private void HideAllIndicators()
    {
        // Hide all indicators initially
        if (EMU1_POWER) EMU1_POWER.SetActive(false);
        if (EV1_SUPPLY) EV1_SUPPLY.SetActive(false);
        if (EV1_WASTE) EV1_WASTE.SetActive(false);
        if (EV2_SUPPLY) EV2_SUPPLY.SetActive(false);
        if (EV2_WASTE) EV2_WASTE.SetActive(false);
        if (EMU2_POWER) EMU2_POWER.SetActive(false);
        if (EMU1_OXY) EMU1_OXY.SetActive(false);
        if (EMU2_OXY) EMU2_OXY.SetActive(false);
        if (O2_VENT) O2_VENT.SetActive(false);
        if (DEPRESS_PUMP) DEPRESS_PUMP.SetActive(false);
    }

    void Update()
    {
        // Check if backend data is available
        var backend = BackendDataService.Instance;
        if (backend == null || backend.LatestData == null || backend.LatestData.eva1 == null)
        {
            if (instructionText) instructionText.text = "Waiting for backend data...";
            return;
        }

        ProcessEgressProcedure();
    }

    private void ProcessEgressProcedure()
    {
        var backend = BackendDataService.Instance;

        // Step 1: Connect UIA to DCU and start Depress
        if (!steps[1].All(x => x.Value == true))
        {
            //UIA and DCU  1.  EV1 and EV2 connect UIA and DCU umbilical
            if (!steps[1][1])
            {
                if (instructionText) instructionText.text = "Connect EV-1 UIA and DCU umbilical.";
                if (EMU1_POWER) EMU1_POWER.SetActive(backend.LatestData.uia.emu1_power < 0.5f);
                if (backend.LatestData.uia.emu1_power < 0.5f) return;
                if (!dcu_batt_msg(true)) return;
                steps[1][1] = true;
            }
            // UIA		2.  EV-1, EV-2 PWR – ON
            if (!steps[1][2])
            {
                if (instructionText) instructionText.text = "Toggle EV-1 PWR on.";
                bool e_pow = backend.LatestData.uia.emu1_power > 0.5f;
                if (EMU1_POWER) EMU1_POWER.SetActive(!e_pow);
                if (!e_pow) return;
                steps[1][2] = true;
            }

            // BOTH DCU	3.  BATT – UMB
            if (!steps[1][3])
            {
                if (!dcu_batt_msg(true)) return;
                steps[1][3] = true;
            }

            // UIA		4.  DEPRESS PUMP PWR – ON
            if (!steps[1][4])
            {
                if (instructionText) instructionText.text = "Toggle depress pump PWR on.";
                if (DEPRESS_PUMP) DEPRESS_PUMP.SetActive(backend.LatestData.uia.depress_pump < 0.5f);
                if (backend.LatestData.uia.depress_pump < 0.5f) return;
                steps[1][4] = true;
            }
        }

        /**
            Step 2. 
Prep O2 Tanks
            
            HMD		2.   Wait until both Primary and Secondary OXY tanks are < 10psi
            UIA		3.    OXYGEN O2 VENT – CLOSE
            BOTH DCU	4.    OXY – PRI
            UIA		5.    OXYGEN EMU-1, EMU-2 – OPEN
            HMD		6.    Wait until EV1 and EV2 Primary O2 tanks > 3000 psi
            UIA		7.    OXYGEN EMU-1, EMU-2 – CLOSE
            BOTH DCU	8.    OXY – SEC
UIA		9.    OXYGEN EMU-1, EMU-2 – OPEN
            HMD		10.  Wait until EV1 and EV2 Secondary O2 tanks > 3000 psi
            UIA		11.  OXYGEN EMU-1, EMU-2 – CLOSE
            BOTH DCU	12.  OXY – PRI

        **/

        if (!steps[2].All(x => x.Value == true))
        {
            // UIA		1.    OXYGEN O2 VENT – OPEN
            if (!steps[2][1])
            {
                if (instructionText) instructionText.text = "Open O2 vent.";
                if (O2_VENT) O2_VENT.SetActive(backend.LatestData.uia.o2_vent < 0.5f);
                if (backend.LatestData.uia.o2_vent < 0.5f) return;

                steps[2][1] = true;
            }

            // HMD		2.   Wait until both Primary and Secondary OXY tanks are < 10psi
            if (!steps[2][2])
            {
                double p1 = backend.LatestData.eva1.telemetry.oxy_pri_storage;
                double s1 = backend.LatestData.eva1.telemetry.oxy_sec_storage;
                if (instructionText) instructionText.text = "eva1 primary: " + p1 + " secondary: " + s1;
                bool empty = p1 < 10 && s1 < 10;
                if (!empty) return;

                steps[2][2] = true;
            }

            // UIA		3.    OXYGEN O2 VENT – CLOSE
            if (!steps[2][3])
            {
                if (instructionText) instructionText.text = "Close O2 vent.";
                if (O2_VENT) O2_VENT.SetActive(backend.LatestData.uia.o2_vent > 0.5f);
                if (backend.LatestData.uia.o2_vent > 0.5f) return;

                steps[2][3] = true;
            }

            // BOTH DCU	4.    OXY – PRI
            if (!steps[2][4])
            {
                if (!dcu_oxy_msg(true)) return;

                steps[2][4] = true;
            }

            // UIA		5.    OXYGEN EMU-1, EMU-2 – OPEN
            if (!steps[2][5])
            {
                if (instructionText) instructionText.text = "Open EMU-1 oxygen.";
                if (EMU1_OXY) EMU1_OXY.SetActive(backend.LatestData.uia.ev1_oxygen < 0.5f);
                if (backend.LatestData.uia.ev1_oxygen < 0.5f) return;

                steps[2][5] = true;
            }

            // HMD		6.    Wait until EV1 and EV2 Primary O2 tanks > 3000 psi
            if (!steps[2][6])
            {
                double p1 = backend.LatestData.eva1.telemetry.oxy_pri_pressure;
                if (instructionText) instructionText.text = "Primary oxygen pressure: " + p1 + ".";
                if (p1 < 3000) return;

                steps[2][6] = true;
            }

            // UIA		7.    OXYGEN EMU-1, EMU-2 – CLOSE
            if (!steps[2][7])
            {
                if (instructionText) instructionText.text = "Close EMU-1 oxygen.";
                if (EMU1_OXY) EMU1_OXY.SetActive(backend.LatestData.uia.ev1_oxygen > 0.5f);
                if (backend.LatestData.uia.ev1_oxygen > 0.5f) return;

                steps[2][7] = true;
            }

            // BOTH DCU	8.    OXY – SEC
            if (!steps[2][8])
            {
                if (!dcu_oxy_msg(false)) return;

                steps[2][8] = true;
            }

            // UIA		9.    OXYGEN EMU-1, EMU-2 – OPEN
            if (!steps[2][9])
            {
                if (instructionText) instructionText.text = "Open EMU-1 oxygen.";
                if (EMU1_OXY) EMU1_OXY.SetActive(backend.LatestData.uia.ev1_oxygen < 0.5f);
                if (backend.LatestData.uia.ev1_oxygen < 0.5f) return;

                steps[2][9] = true;
            }

            // HMD  	10.  Wait until EV1 and EV2 Secondary O2 tanks > 3000 psi
            if (!steps[2][10])
            {
                double p1 = backend.LatestData.eva1.telemetry.oxy_sec_pressure;
                if (instructionText) instructionText.text = "Secondary oxygen pressure: " + p1 + ".";
                if (p1 < 3000) return;

                steps[2][10] = true;
            }

            // UIA		11.  OXYGEN EMU-1, EMU-2 – CLOSE
            if (!steps[2][11])
            {
                if (instructionText) instructionText.text = "Close EMU-1 oxygen.";
                if (EMU1_OXY) EMU1_OXY.SetActive(backend.LatestData.uia.ev1_oxygen > 0.5f);
                if (backend.LatestData.uia.ev1_oxygen > 0.5f) return;

                steps[2][11] = true;
            }

            // BOTH DCU	12.  OXY – PRI
            if (!steps[2][12])
            {
                if (!dcu_oxy_msg(true)) return;

                steps[2][12] = true;
            }
        }

        /**
            Step 3.
Prep Water Tanks
            BOTH DCU	1.  PUMP – OPEN
            UIA		2.  EV-1, EV-2 WASTE WATER – OPEN
            HMD 		3.  Wait until water EV1 and EV2 Coolant tank is < 5%
            UIA		4.  EV-1, EV-2 WASTE WATER – CLOSE
            UIA		5.  EV-1, EV-2 SUPPLY WATER – OPEN
            HMD		6.  Wait until water EV1 and EV2 Coolant tank is > 95%
            UIA		7.  EV-1, EV-2 SUPPLY WATER – CLOSE
            BOTH DCU	8.  PUMP – CLOSE

        **/
        if (!steps[3].All(x => x.Value == true))
        {
            // BOTH DCU	1.  PUMP – OPEN
            if (!steps[3][1])
            {
                if (!dcu_pump_msg(true)) return;

                steps[3][1] = true;
            }

            // UIA		2.  EV-1, EV-2 WASTE WATER – OPEN
            if (!steps[3][2])
            {
                if (instructionText) instructionText.text = "Open EV-1 waste water.";
                if (EV1_WASTE) EV1_WASTE.SetActive(backend.LatestData.uia.ev1_waste < 0.5f);
                if (backend.LatestData.uia.ev1_waste < 0.5f) return;

                steps[3][2] = true;
            }

            // HMD 	3.  Wait until water EV1 and EV2 Coolant tank is < 5%
            if (!steps[3][3])
            {
                double p1 = backend.LatestData.eva1.telemetry.coolant_ml;
                if (instructionText) instructionText.text = "Coolant (mL): " + p1 + ".";
                if (p1 < 5) return;

                steps[3][3] = true;
            }

            // UIA		4.  EV-1, EV-2 WASTE WATER – CLOSE
            if (!steps[3][4])
            {
                if (instructionText) instructionText.text = "Close EV-1 waste water.";
                if (EV1_WASTE) EV1_WASTE.SetActive(backend.LatestData.uia.ev1_waste > 0.5f);

                if (backend.LatestData.uia.ev1_waste > 0.5f) return;
                steps[3][4] = true;
            }

            // UIA		5.  EV-1, EV-2 SUPPLY WATER – OPEN
            if (!steps[3][5])
            {
                if (instructionText) instructionText.text = "Open EV-1 supply water.";
                if (EV1_SUPPLY) EV1_SUPPLY.SetActive(backend.LatestData.uia.ev1_supply < 0.5f);
                steps[3][5] = true;
            }

            // HMD		6.  Wait until water EV1 and EV2 Coolant tank is > 95%
            if (!steps[3][6])
            {
                double p1 = backend.LatestData.eva1.telemetry.coolant_ml;
                if (instructionText) instructionText.text = "Coolant (mL): " + p1 + ".";
                if (p1 < 95) return;
                steps[3][6] = true;
            }

            // UIA		7.  EV-1, EV-2 SUPPLY WATER – CLOSE
            if (!steps[3][7])
            {
                if (instructionText) instructionText.text = "Close EV-1 supply water.";
                if (EV1_SUPPLY) EV1_SUPPLY.SetActive(backend.LatestData.uia.ev1_supply > 0.5f);
                if (backend.LatestData.uia.ev1_supply > 0.5f) return;
                steps[3][7] = true;
            }

            // BOTH DCU	8.  PUMP – CLOSE
            if (!steps[3][8])
            {
                if (!dcu_pump_msg(false)) return;
                steps[3][8] = true;
            }
        }

        /**
            Step 4.
END Depress, Check Switches and Disconnect
            HMD		1.  Wait until SUIT P, O2 P = 4
            UIA		2.  DEPRESS PUMP PWR – OFF
            BOTH DCU	3.  BATT – LOCAL
UIA		9.   EV-1, EV-2 PWR - OFF
            BOTH DCU	4.  Verify OXY – PRI
BOTH DCU	5.  Verify COMMS – A
BOTH DCU 	6.  Verify FAN – PRI
BOTH DCU	7.  Verify PUMP – CLOSE
BOTH DCU	8.  Verify CO2 – A
UIA and DCU  9.  EV1 and EV2 disconnect UIA and DCU umbilical
        **/
        if (!steps[4].All(x => x.Value == true))
        {
            // HMD		1.  Wait until SUIT P, O2 P = 4
            if (!steps[4][1])
            {
                double suit_p = backend.LatestData.eva1.telemetry.suit_pressure_total;
                double suit_o2 = backend.LatestData.eva1.telemetry.suit_pressure_oxy;
                if (instructionText) instructionText.text = "Suit pressure: " + suit_p + " O2 pressure: " + suit_o2 + ".";
                if (!((suit_p - 4) < .01 && (suit_o2 - 4) < .01)) return;
                steps[4][1] = true;
            }

            // UIA		2.  DEPRESS PUMP PWR – OFF
            if (!steps[4][2])
            {
                if (instructionText) instructionText.text = "Toggle depress pump PWR off.";
                if (DEPRESS_PUMP) DEPRESS_PUMP.SetActive(backend.LatestData.uia.depress_pump > 0.5f);
                if (backend.LatestData.uia.depress_pump > 0.5f) return;
                steps[4][2] = true;
            }

            // BOTH DCU	3.  BATT – LOCAL
            if (!steps[4][3])
            {
                if (!dcu_batt_msg(false)) return;
                steps[4][3] = true;
            }

            // UIA		4.   EV-1, EV-2 PWR - OFF
            if (!steps[4][4])
            {
                if (instructionText) instructionText.text = "Toggle EV-1 PWR off.";
                if (EMU1_POWER) EMU1_POWER.SetActive(backend.LatestData.uia.emu1_power > 0.5f);
                if (backend.LatestData.uia.emu1_power > 0.5f) return;
                steps[4][4] = true;
            }

            // BOTH DCU	5.  Verify OXY – PRI
            if (!steps[4][5])
            {
                if (!dcu_oxy_msg(true)) return;
                steps[4][5] = true;
            }

            // BOTH DCU	6.  Verify COMMS – A
            if (!steps[4][6])
            {
                if (!dcu_comm_msg(true)) return;
                steps[4][6] = true;
            }

            // BOTH DCU 7.  Verify FAN – PRI
            if (!steps[4][7])
            {
                if (!dcu_fan_msg(true)) return;
                steps[4][7] = true;
            }

            // BOTH DCU	8.  Verify PUMP – CLOSE
            if (!steps[4][8])
            {
                if (!dcu_pump_msg(false)) return;
                steps[4][8] = true;
            }

            // BOTH DCU	9.  Verify CO2 – A
            if (!steps[4][9])
            {
                if (!dcu_co2_msg(true)) return;
                steps[4][9] = true;
            }

            // UIA and DCU  10.  EV1 and EV2 disconnect UIA and DCU umbilical
            if (!steps[4][10])
            {
                if (instructionText) instructionText.text = "Disconnect EV-1 UIA and DCU umbilical.";
                if (EMU1_POWER) EMU1_POWER.SetActive(backend.LatestData.uia.emu1_power < 0.5f);
                if (backend.LatestData.uia.emu1_power < 0.5f) return;
                if (!dcu_batt_msg(false)) return;
                steps[4][10] = true;
                duringEVA = true;
            }

            if(duringEVA == true){
                if (instructionText) instructionText.text = "Egress Complete!";
            }
        }
    }

    bool dcu_batt_msg(bool mode)
    {
        var backend = BackendDataService.Instance;
        string modeText = mode ? "umbilical" : "local";
        bool currentMode = backend.LatestData.eva1.dcu.battery > 0.5f;
        if (currentMode != mode)
        {
            if (instructionText) instructionText.text = "Switch DCU batt to " + modeText + ". ";
            return false;
        }
        return true;
    }
    
    bool dcu_oxy_msg(bool mode)
    {
        var backend = BackendDataService.Instance;
        string modeText = mode ? "primary" : "secondary";
        bool currentMode = backend.LatestData.eva1.dcu.oxygen > 0.5f;
        if (currentMode != mode)
        {
            if (instructionText) instructionText.text = "Switch DCU oxy to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_comm_msg(bool mode)
    {
        var backend = BackendDataService.Instance;
        string modeText = mode ? "A" : "B";
        bool currentMode = backend.LatestData.eva1.dcu.comms > 0.5f;
        if (currentMode != mode)
        {
            if (instructionText) instructionText.text = "Switch DCU comm to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_fan_msg(bool mode)
    {
        var backend = BackendDataService.Instance;
        string modeText = mode ? "primary" : "secondary";
        bool currentMode = backend.LatestData.eva1.dcu.fan > 0.5f;
        if (currentMode != mode)
        {
            if (instructionText) instructionText.text = "Switch DCU fan to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_pump_msg(bool mode)
    {
        var backend = BackendDataService.Instance;
        string modeText = mode ? "open" : "closed";
        bool currentMode = backend.LatestData.eva1.dcu.pump > 0.5f;
        if (currentMode != mode)
        {
            if (instructionText) instructionText.text = "Switch DCU pump to " + modeText + ". ";
            return false;
        }
        return true;
    }

    bool dcu_co2_msg(bool mode)
    {
        var backend = BackendDataService.Instance;
        string modeText = mode ? "A" : "B";
        bool currentMode = backend.LatestData.eva1.dcu.co2 > 0.5f;
        if (currentMode != mode)
        {
            if (instructionText) instructionText.text = "Switch DCU co2 to " + modeText + ". ";
            return false;
        }
        return true;
    }
}