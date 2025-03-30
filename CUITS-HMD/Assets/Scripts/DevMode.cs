using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMode : MonoBehaviour
{
    [Header("Dev Mode Settings")]
    public bool devModeEnabled = false;
    public GameObject[] menuPanels;
    public Vector3[] devModePositions;
    public Vector3[] devModeRotations;
    
    // Store original positions and rotations
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;
    private bool[] originalActiveStates;

    private void Start()
    {
        // Initialize arrays to store original transforms
        originalPositions = new Vector3[menuPanels.Length];
        originalRotations = new Quaternion[menuPanels.Length];
        originalActiveStates = new bool[menuPanels.Length];
        
        // Store original positions and rotations
        for (int i = 0; i < menuPanels.Length; i++)
        {
            if (menuPanels[i] != null)
            {
                originalPositions[i] = menuPanels[i].transform.position;
                originalRotations[i] = menuPanels[i].transform.rotation;
                originalActiveStates[i] = menuPanels[i].activeSelf;
            }
        }
        
        // Apply dev mode if enabled at start
        if (devModeEnabled)
        {
            EnableDevMode();
        }
    }

    private void Update()
    {
        // Toggle dev mode with F1 key for testing in editor
        if (Input.GetKeyDown(KeyCode.F1))
        {
            devModeEnabled = !devModeEnabled;
            if (devModeEnabled)
            {
                EnableDevMode();
            }
            else
            {
                DisableDevMode();
            }
        }
    }

    public void EnableDevMode()
    {
        devModeEnabled = true;
        for (int i = 0; i < menuPanels.Length; i++)
        {
            if (menuPanels[i] != null && i < devModePositions.Length)
            {
                // Make menu visible
                menuPanels[i].SetActive(true);
                
                // Set to dev mode position and rotation
                menuPanels[i].transform.position = devModePositions[i];
                menuPanels[i].transform.eulerAngles = devModeRotations[i];
            }
        }
        
        Debug.Log("Dev Mode Enabled - Menus positioned for easy testing");
    }

    public void DisableDevMode()
    {
        devModeEnabled = false;
        for (int i = 0; i < menuPanels.Length; i++)
        {
            if (menuPanels[i] != null)
            {
                // Restore original positions, rotations and visibility
                menuPanels[i].transform.position = originalPositions[i];
                menuPanels[i].transform.rotation = originalRotations[i];
                menuPanels[i].SetActive(originalActiveStates[i]);
            }
        }
        
        Debug.Log("Dev Mode Disabled - Regular hand menu behavior restored");
    }
} 