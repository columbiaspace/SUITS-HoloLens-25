using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // if you use Text
using TMPro; // if you use TextMeshProUGUI

public class UIManager : MonoBehaviour
{
    // Update is called once per frame
    // void Update()
    // {
    // }
    public GameObject MainMenuObj;
    public GameObject AnalysisScreenObj;
    public GameObject ViewDatabaseObj;

    public TextMeshProUGUI analysisOutputText; // Assign this in the inspector
    private RockAnalyzer analyzer;

    private void Start()
    {
        analyzer = GetComponent<RockAnalyzer>();
        showMainScreen();
    }

    public void hideAllScreens(){
        MainMenuObj.SetActive(false);
        AnalysisScreenObj.SetActive(false);
        ViewDatabaseObj.SetActive(false);
    }

    public void showMainScreen(){
        hideAllScreens();
        MainMenuObj.SetActive(true);
    }

    public void showDatabaseScreen(){
        hideAllScreens();
        ViewDatabaseObj.SetActive(true);
    }

    public void showRockAnalysisScreen()
    {
        Debug.Log("Run Analysis Button Clicked");
        StartCoroutine(showAnalysisScreenCoroutine()); // <-- StartCoroutine required
    }

    public IEnumerator showAnalysisScreenCoroutine()
    {
        hideAllScreens();

        Debug.Log("Starting Rock Analysis...");

        // Fetch rock data
        var results = analyzer.AnalyzeRock();

        Debug.Log("Finished fetching data from server.");

        yield return new WaitForSeconds(3.0f);

        // Build output string
        string output = "EVA1 chemical composition:\n";
        foreach (var kvp in results)
        {
            output += $"{kvp.Key}: {kvp.Value}\n";
            Debug.Log($"{kvp.Key}: {kvp.Value}"); 
        }

        // Set the text field
        analysisOutputText.text = output;

        // Now show the analysis screen
        AnalysisScreenObj.SetActive(true);
    }


}
