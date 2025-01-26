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
using System;
using System.Xml;

public class evaTime : MonoBehaviour
{
   public TSS_DATA TSS;
   public TSScConnection TSSc;
   public TMP_Text display; 
   public TMP_Text check;

   int minutes;
   int seconds;
   int time_in_secs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(TSSc.connected ==true){
       time_in_secs = TSS.tel.telemetry.eva_time;
       minutes = time_in_secs / 60;
       seconds = time_in_secs % 60;
       if(seconds >= 10){
       display.text = minutes.ToString() + ":" + seconds.ToString(); 
       }else{
       display.text = minutes.ToString() + ":"  + "0" + seconds.ToString() ;
       }
       }
    }
}
