using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Procedure_Display : MonoBehaviour
{
    public LMCC_CONNECT lmcc;
    public TMP_Text Title;
    public TMP_Text Body;
    int counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Title.text = lmcc.GoldenProcedure.title;
        Body.text = "";
        Body.text += lmcc.GoldenProcedure.steps[counter].step;
        Body.text += " (";
        Body.text += lmcc.GoldenProcedure.steps[counter].role;
        Body.text += "): ";
        Body.text += lmcc.GoldenProcedure.steps[counter].description;

    }

    public void nextStep(){
            ++counter;
            Body.text += lmcc.GoldenProcedure.steps[counter].step;
            Body.text += " (";
            Body.text += lmcc.GoldenProcedure.steps[counter].role;
            Body.text += "): ";
            Body.text += lmcc.GoldenProcedure.steps[counter].description;
    }

    public void prevStep()
    {
        if (counter >= 1)
        {


            --counter;
            Body.text += lmcc.GoldenProcedure.steps[counter].step;
            Body.text += " (";
            Body.text += lmcc.GoldenProcedure.steps[counter].role;
            Body.text += "): ";
            Body.text += lmcc.GoldenProcedure.steps[counter].description;
        }
    }



}
