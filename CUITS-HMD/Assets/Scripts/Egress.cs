using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Egress : MonoBehaviour
{
    //set of game objects that are indicators to toggle buttons based on data in tss_obj 
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
    Dictionary<int, bool> step5 = new Dictionary<int, bool>();
    Dictionary<int, bool> step6 = new Dictionary<int, bool>();
    Dictionary<int, bool> step7 = new Dictionary<int, bool>();
    Dictionary<int, bool> step8 = new Dictionary<int, bool>();
    // Start is called before the first frame update
    void Start()
    {
        // Game Object stickers to show users what to toggle
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

    // Update is called once per frame
    void Update()
    {

        /** ALL STEPS 
    Step 1.
Connect UIA to DCU and start Depress
UIA and DCU  1.  EV1 and EV2 connect UIA and DCU umbilical
	UIA		2.  EV-1, EV-2 PWR – ON
	BOTH DCU	3.  BATT – UMB
	UIA		4.  DEPRESS PUMP PWR – ON

    Step 2. 
Prep O2 Tanks
	UIA		1.    OXYGEN O2 VENT – OPEN
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

        /**
            Step 1.
Connect UIA to DCU and start Depress
UIA and DCU  1.  EV1 and EV2 connect UIA and DCU umbilical
            UIA		2.  EV-1, EV-2 PWR – ON
            BOTH DCU	3.  BATT – UMB
            UIA		4.  DEPRESS PUMP PWR – ON
        **/
        
        if (!steps[1].All(x => x.Value == true))
        {
            //UIA and DCU  1.  EV1 and EV2 connect UIA and DCU umbilical
            if (!steps[1][1])
            {
                text.text = "Connect EV-1 UIA and DCU umbilical.";
                EMU1_POWER.SetActive(!tss_obj.uia.uia.eva1_power);
                if (!tss_obj.uia.uia.eva1_power) return;
                if (!dcu_batt_msg(true)) return;
                steps[1][1] = true;
            }
            // UIA		2.  EV-1, EV-2 PWR – ON
            if (!steps[1][2])
            {
                text.text = "Toggle EV-1 PWR on.";
                bool e_pow = tss_obj.uia.uia.eva1_power;
                EMU1_POWER.SetActive(!e_pow);
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
                text.text = "Toggle depress pump PWR on.";
                DEPRESS_PUMP.SetActive(!tss_obj.uia.uia.depress);
                if (!tss_obj.uia.uia.depress) return;
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
                text.text = "Open O2 vent.";
                O2_VENT.SetActive(!tss_obj.uia.uia.oxy_vent);
                if (!tss_obj.uia.uia.oxy_vent) return;

                steps[2][1] = true;
            }

            // HMD		2.   Wait until both Primary and Secondary OXY tanks are < 10psi
            if (!steps[2][2])
            {
                double p1 = tss_obj.tel.telemetry.eva1.oxy_pri_storage;
                double s1 = tss_obj.tel.telemetry.eva1.oxy_sec_storage;
                text.text = "eva1 primary: " + p1 + " secondary: " + s1;//+ " eva2 primary: " + p2 + " secondary: " + s2 + ".";
                bool empty = p1 < 10 && s1 < 10; //&& p2 == 0 && s2 == 0;
                if (!empty) return;

                steps[2][2] = true;
            }

            // UIA		3.    OXYGEN O2 VENT – CLOSE
            if (!steps[2][3])
            {
                text.text = "Close O2 vent.";
                O2_VENT.SetActive(tss_obj.uia.uia.oxy_vent);
                if (tss_obj.uia.uia.oxy_vent) return;

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
                text.text = "Open EMU-1 oxygen.";
                EMU1_OXY.SetActive(!tss_obj.uia.uia.eva1_oxy);
                if (!tss_obj.uia.uia.eva1_oxy) return;

                steps[2][5] = true;
            }

            // HMD		6.    Wait until EV1 and EV2 Primary O2 tanks > 3000 psi
            if (!steps[2][6])
            {
                double p1 = tss_obj.tel.telemetry.eva1.oxy_pri_pressure;
                text.text = "Primary oxygen pressure: " + p1 + ".";
                if (p1 < 3000) return;

                steps[2][6] = true;
            }

            // UIA		7.    OXYGEN EMU-1, EMU-2 – CLOSE
            if (!steps[2][7])
            {
                text.text = "Close EMU-1 oxygen.";
                EMU1_OXY.SetActive(tss_obj.uia.uia.eva1_oxy);
                if (tss_obj.uia.uia.eva1_oxy) return;

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
                text.text = "Open EMU-1 oxygen.";
                EMU1_OXY.SetActive(!tss_obj.uia.uia.eva1_oxy);
                if (!tss_obj.uia.uia.eva1_oxy) return;

                steps[2][9] = true;
            }

            // HMD  	10.  Wait until EV1 and EV2 Secondary O2 tanks > 3000 psi
            if (!steps[2][10])
            {
                double p1 = tss_obj.tel.telemetry.eva1.oxy_sec_pressure;
                text.text = "Secondary oxygen pressure: " + p1 + ".";
                if (p1 < 3000) return;

                steps[2][10] = true;
            }

            // UIA		11.  OXYGEN EMU-1, EMU-2 – CLOSE
            if (!steps[2][11])
            {
                text.text = "Close EMU-1 oxygen.";
                EMU1_OXY.SetActive(tss_obj.uia.uia.eva1_oxy);
                if (tss_obj.uia.uia.eva1_oxy) return;

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
                text.text = "Open EV-1 waste water.";
                EV1_WASTE.SetActive(!tss_obj.uia.uia.eva1_water_waste);
                if (!tss_obj.uia.uia.eva1_water_waste) return;

                steps[3][2] = true;
            }

            // HMD 	3.  Wait until water EV1 and EV2 Coolant tank is < 5%
            if (!steps[3][3])
            {
                double p1 = tss_obj.tel.telemetry.eva1.coolant_ml;
                text.text = "Coolant (mL): " + p1 + ".";
                if (p1 < 5) return;

                steps[3][3] = true;
            }

            // UIA		4.  EV-1, EV-2 WASTE WATER – CLOSE
            if (!steps[3][4])
            {
                text.text = "Close EV-1 waste water.";
                EV1_WASTE.SetActive(tss_obj.uia.uia.eva1_water_waste);

                if (tss_obj.uia.uia.eva1_water_waste) return;
                steps[3][4] = true;
            }

            // UIA		5.  EV-1, EV-2 SUPPLY WATER – OPEN
            if (!steps[3][5])
            {
                text.text = "Open EV-1 supply water.";
                EV1_SUPPLY.SetActive(!tss_obj.uia.uia.eva1_water_supply);
                steps[3][5] = true;
            }

            // HMD		6.  Wait until water EV1 and EV2 Coolant tank is > 95%
            if (!steps[3][6])
            {
                double p1 = tss_obj.tel.telemetry.eva1.coolant_ml;
                text.text = "Coolant (mL): " + p1 + ".";
                if (p1 < 95) return;
                steps[3][6] = true;
            }

            // UIA		7.  EV-1, EV-2 SUPPLY WATER – CLOSE
            if (!steps[3][7])
            {
                text.text = "Close EV-1 supply water.";
                EV1_SUPPLY.SetActive(tss_obj.uia.uia.eva1_water_supply);
                if (tss_obj.uia.uia.eva1_water_supply) return;
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
                double suit_p = tss_obj.tel.telemetry.eva1.suit_pressure_total;
                double suit_o2 = tss_obj.tel.telemetry.eva1.suit_pressure_oxy;
                text.text = "Suit pressure: " + suit_p + " O2 pressure: " + suit_o2 + ".";
                if (!((suit_p - 4) < .01 && (suit_o2 - 4) < .01)) return;
                steps[4][1] = true;
            }

            // UIA		2.  DEPRESS PUMP PWR – OFF
            if (!steps[4][2])
            {
                text.text = "Toggle depress pump PWR off.";
                DEPRESS_PUMP.SetActive(tss_obj.uia.uia.depress);
                if (tss_obj.uia.uia.depress) return;
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
                text.text = "Toggle EV-1 PWR off.";
                EMU1_POWER.SetActive(tss_obj.uia.uia.eva1_power);
                if (tss_obj.uia.uia.eva1_power) return;
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
                text.text = "Disconnect EV-1 UIA and DCU umbilical.";
                EMU1_POWER.SetActive(!tss_obj.uia.uia.eva1_power);
                if (!tss_obj.uia.uia.eva1_power) return;
                if (!dcu_batt_msg(false)) return;
                steps[4][10] = true;
                tss_obj.duringEVA = true;
            }

            if(tss_obj.duringEVA == true){
                text.text = "Egress Complete!";
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