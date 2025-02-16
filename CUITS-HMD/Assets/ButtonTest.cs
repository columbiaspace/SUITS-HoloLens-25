using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ButtonTest : MonoBehaviour
{
    public TextMeshProUGUI numberText; // TextMeshPro object
    int counter;
    // Increments counter everytime ButtonPress() is called
    public void ButtonPress()
    {
        counter++;
        // Assign the value of counter to numberText
        numberText.text = counter + ""; // + "" makes it a string
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
}