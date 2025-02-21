using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropPin : MonoBehaviour
{
    public GameObject[] pins; // Array to store all the objects (pins) you want to appear
    public Vector3[] positions; // Array to store the positions for each pin
    private int pressCount = 0; // Counter to track how many times the button has been pressed

    // Increments counter every time ButtonPress() is called
    public void ButtonPress()
    {
        // Check if we have pins and positions arrays, and if the counter is within range
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

    // Start is called before the first frame update
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


// public class DropPin : MonoBehaviour
// {
//     public GameObject pin; // Reference to the pin sprite (the object you want to control)

//     // Increments counter every time ButtonPress() is called
//     public void ButtonPress()
//     {
//         // Check if the pin exists
//         if (pin != null)
//         {
//             // Make the pin visible (if it's not already) by setting active to true
//             pin.SetActive(true);

//             // Move the pin to the new position
//             pin.transform.localPosition = new Vector3(0, 0, -0.05f);
//         }
//     }
    
//     // Start is called before the first frame update
//     void Start()
//     {
//         if (pin != null)
//         {
//             // Set the pin to be invisible at the start
//             pin.SetActive(false); // Initially hide the pin
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // You could add other code to handle input or animations here if needed
//     }
// }
