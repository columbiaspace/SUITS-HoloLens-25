using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropPin : MonoBehaviour
{
    public GameObject[] pins; // Array to store all the objects (pins) you want to appear
    private Vector3 latestPosition; // Single latest position instead of array

    private int pressCount = 0; // Counter to track how many times the button has been pressed

    // Set positions dynamically (used in APIClient)
    public void SetPosition(Vector3 newPosition)
    {
        latestPosition = newPosition;
    }

    public void ButtonPress()
    {
        if (pins != null && pressCount < pins.Length)
        {
            pins[pressCount].SetActive(true);
            pins[pressCount].transform.localPosition = latestPosition;
            pressCount++;
        }
    }

    void Start()
    {
        // Initially hide all pins
        if (pins != null)
        {
            foreach (var pin in pins)
            {
                pin.SetActive(false);
            }
        }
    }
}
