using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropPin : MonoBehaviour
{
    public GameObject pin; // Reference to the pin sprite (the object you want to control)

    // Increments counter every time ButtonPress() is called
    public void ButtonPress()
    {
        // Check if the pin exists
        if (pin != null)
        {
            // Make the pin visible (if it's not already) by setting active to true
            pin.SetActive(true);

            // Move the pin to the new position
            pin.transform.localPosition = new Vector3(0, 0, -0.05f);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (pin != null)
        {
            // Set the pin to be invisible at the start
            pin.SetActive(false); // Initially hide the pin
        }
    }

    // Update is called once per frame
    void Update()
    {
        // You could add other code to handle input or animations here if needed
    }
}
