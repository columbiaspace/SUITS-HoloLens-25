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

public class TSS_Serialized : MonoBehaviour
{
    
    // TSSc Connection
    public TSScConnection TSSc;
    public LMCC_CONNECT LMCCc;

    // UI Input
    public TMP_InputField InputFieldUrl;

    
    //Status displays
    public TMP_Text status;
    public TMP_Text ip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    // On Connect Button Press
    public void Connect_Button()
    {
        // Get URL in Text Field
        string host = InputFieldUrl.text;

        LMCCc.ConnectToHost(host);
        if(LMCCc.connected == true){
            ip.text = "LMCC: Connected";
            // Print Hostname to Logs
            Debug.Log("Button Pressed: " + host);
            string tss_ip = LMCCc.the_config.TSS_IP;

            // Connect to TSSc at that Host
            TSSc.ConnectToHost(tss_ip, Int32.Parse(LMCCc.the_config.EV1_TEAM_ID));
            if (TSSc.connected == true)
            {
                status.text = "TSS: Connected";
            }
        }
    }

    public void Disconnect_Button()
    {
        // Disconnects from TSS when
        ip.text = "LMCC: Not Connected";
        status.text = "TSS: Not Connected";
        TSSc.DisconnectFromHost();
        LMCCc.DisconnectFromHost();
    }

}
