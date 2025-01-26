using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class REALROCK : MonoBehaviour
{
    public TSS_DATA TSS;
    public TMP_Text display;
    bool signif = false;
   

    // Start is called before the first frame update
    void Start()
    {
        //display.text = "Please collect a rock sample. \n\nThe rock data will be transmitted here upon scanning";
    }

    IEnumerator NormMessage()
    {
        display.text = "Rock scanned successfully.";
        yield return new WaitForSeconds(3);
        display.text = "";
    }

    IEnumerator FlagMessage()
    {
        display.text = "Rock scanned successfully.\nRock composition outside nominal parameters.";
        yield return new WaitForSeconds(3);
        display.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // If the rock id value isn't set to default
        if (TSS.spec.spec.eva1.id != 0 ) {
            if ( TSS.spec.spec.eva1.data.SiO2 < 10 ||  TSS.spec.spec.eva1.data.TiO2 > 1 ||  TSS.spec.spec.eva1.data.Al2O3 > 10 || 
                 TSS.spec.spec.eva1.data.FeO > 29 ||  TSS.spec.spec.eva1.data.MnO > 1 || TSS.spec.spec.eva1.data.MgO > 20 || 
                 TSS.spec.spec.eva1.data.CaO > 10 ||  TSS.spec.spec.eva1.data.K2O > 1 ||  TSS.spec.spec.eva1.data.P2O3 > 1.5) {
                StartCoroutine(FlagMessage());
            } else {
                StartCoroutine(NormMessage());
            }
            // Set rock id back to default
            TSS.spec.spec.eva1.id = 0;
        }
    }
}
