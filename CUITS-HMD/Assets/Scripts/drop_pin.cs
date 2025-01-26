using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drop_pin : MonoBehaviour
{
    Dictionary<int, string> pins = new Dictionary<int, string>();
    int num_pins = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pin_Drop(string str)
    {
        pins.Add(++num_pins, str);
    }
}
