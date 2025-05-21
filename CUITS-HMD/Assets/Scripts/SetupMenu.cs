using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // For Button and InputField
using Unity.Profiling;

// Global variables for backend IP and EV number
public static class Globals
{
    public static string backendIP = "127.0.0.1";
    public static string evNum = "1";
}

public class SetupMenu : MonoBehaviour
{
    public TMP_InputField backendIPInputField;
    public TMP_InputField evNumInputField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButton()
    {
        // pull the current text from each field...
        string backendIPText  = backendIPInputField.text;
        string evNumText = evNumInputField.text;

        // ...and stash it in your global class
        Globals.backendIP  = backendIPText;
        Globals.evNum = evNumText;

        Debug.Log($"Globals set: BackendIP={Globals.backendIP}, evNum={Globals.evNum}");
    }
}
