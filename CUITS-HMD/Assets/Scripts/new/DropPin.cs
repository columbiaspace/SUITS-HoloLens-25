using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropPin : MonoBehaviour
{
    public GameObject[] pins; // Array to store all the objects (pins) you want to appear
    private Vector3[] positions; // Private array to store positions for each pin

    private int pressCount = 0; // Counter to track how many times the button has been pressed

    // Set positions dynamically (used in APIClient)
    public void SetPositions(Vector3[] newPositions)
    {
        positions = newPositions; // Update positions with the data from FastAPI
    }

    public void ButtonPress()
    {
        if (pins != null && positions != null && pressCount < pins.Length && pressCount < positions.Length)
        {
            // Make the corresponding pin visible based on pressCount
            pins[pressCount].SetActive(true);

            // Move the pin to the new position from the positions array
            pins[pressCount].transform.localPosition = positions[pressCount];

            // Increment the press count for the next time the button is pressed
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
