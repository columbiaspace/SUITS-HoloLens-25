// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Microsoft.MixedReality.Toolkit.UI;
// using TMPro;

// public class DropPin : MonoBehaviour
// {
//     [SerializeField] public GameObject pin;
//     public TextMeshProUGUI test;

//     // Start is called before the first frame update
//     void Start()
//     {
//         if (pin != null)
//         {
//             pin.SetActive(true); // You can start with it visible
//         }
        
//     }

//     public void ButtonPress()
//     {
//         // Toggle the sprite visibility on button press
//         if (pin != null)
//         {
//             pin.SetActive(!pin.activeSelf); // Toggle visibility
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropPin : MonoBehaviour
{

    public GameObject pin;

    // Increments counter everytime ButtonPress() is called
    public void ButtonPress()
    {
        // Toggle the sprite visibility on button press
        if (pin != null)
        {
            pin.SetActive(!pin.activeSelf); // Toggle visibility
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (pin != null)
        {
            pin.SetActive(true); // You can start with it visible
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
