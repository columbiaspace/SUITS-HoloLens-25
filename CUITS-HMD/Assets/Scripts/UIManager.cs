using System.Collections;
using System.Collections.Generic;
using System.IO; // <-- For saving files
using System.Linq; // <-- For using Max()
using Newtonsoft.Json; // <-- For JSON serialization (you might need to install it)
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UIManager : MonoBehaviour
{
    // Update is called once per frame
    // void Update()
    // {
    // }
    public GameObject MainMenuObj;
    public GameObject AnalysisScreenObj;
    public GameObject ViewDatabaseObj;
    public GameObject AddNotesObj;

    // Input fields
    public TMP_InputField OtherNotesInput;
    public TMP_Dropdown ColorDrop;
    public TMP_Dropdown TextureDrop;
    public TMP_Dropdown SizeDrop;

    //output text that is rendered
    public TextMeshProUGUI analysisOutputText; // Assign this in the inspector
    private RockAnalyzer analyzer;
    public TextMeshProUGUI databaseOutputText;


    private List<RockAnalysisEntry> rockAnalysisDatabase = new List<RockAnalysisEntry>();
    private string saveFilePath;
    private RockAnalysisEntry currentEntryBeingEdited; // Store current entry

    // private void Start()
    // {
    //     analyzer = GetComponent<RockAnalyzer>();
    //     showMainScreen();
    // }

    private void Start()
    {
        analyzer = GetComponent<RockAnalyzer>();
        saveFilePath = Path.Combine(Application.persistentDataPath, "rock_analysis_database.json");
        Debug.Log(Application.persistentDataPath);

        LoadDatabase();
        showMainScreen();
    }

    private void SaveDatabase()
    {
        string json = JsonConvert.SerializeObject(rockAnalysisDatabase, Formatting.Indented);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Database saved to {saveFilePath}");
    }

    private void LoadDatabase()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            rockAnalysisDatabase = JsonConvert.DeserializeObject<List<RockAnalysisEntry>>(json);
            Debug.Log($"Database loaded with {rockAnalysisDatabase.Count} entries.");
        }
        else
        {
            Debug.Log("No database file found, starting new.");
            rockAnalysisDatabase = new List<RockAnalysisEntry>();
        }
    }


    public void hideAllScreens(){
        MainMenuObj.SetActive(false);
        AnalysisScreenObj.SetActive(false);
        ViewDatabaseObj.SetActive(false);
        AddNotesObj.SetActive(false);
    }

    public void showMainScreen(){
        Debug.Log("Back button pressed");
        hideAllScreens();
        MainMenuObj.SetActive(true);
    }

    public void showAddNotesScreen(){
        Debug.Log("Add Notes button pressed.");
        hideAllScreens();
        AddNotesObj.SetActive(true);

        //clear fields
        ColorDrop.value = 0;
        TextureDrop.value = 0;
        SizeDrop.value = 0;
        OtherNotesInput.text = "";
    }

    public void submitNotes()
    {
        if (currentEntryBeingEdited != null)
        {
            currentEntryBeingEdited.color = ColorDrop.options[ColorDrop.value].text;
            currentEntryBeingEdited.texture = TextureDrop.options[TextureDrop.value].text;
            currentEntryBeingEdited.size = SizeDrop.options[SizeDrop.value].text;
            currentEntryBeingEdited.otherNotes = OtherNotesInput.text;


            SaveDatabase();
        }
        else
        {
            Debug.LogWarning("No current entry being edited!");
        }

        // Go back to the Run Analysis screen
        hideAllScreens();
        AnalysisScreenObj.SetActive(true);
    }



    // public void showDatabaseScreen(){
    //     hideAllScreens();
    //     ViewDatabaseObj.SetActive(true);
    // }

    public void showDatabaseScreen()
    {
        hideAllScreens();
        ViewDatabaseObj.SetActive(true);

        string databaseOutput = "";

        foreach (var entry in rockAnalysisDatabase)
        {
            databaseOutput += $"Analysis ID: {entry.id}\n";
            databaseOutput += $"Timestamp: {entry.timestamp}\n";
            
            // Show Chemical Composition
            foreach (var kvp in entry.chemicalData)
            {
                databaseOutput += $"{kvp.Key}: {kvp.Value}\n";
            }

            // Show Astronaut Notes
            databaseOutput += $"Color: {entry.color}\n";
            databaseOutput += $"Size: {entry.size}\n";
            databaseOutput += $"Texture: {entry.texture}\n";
            databaseOutput += $"Other Notes: {entry.otherNotes}\n";
            
            databaseOutput += "\n";
        }


        if (databaseOutputText != null)
        {
            databaseOutputText.text = databaseOutput;
        }
        else
        {
            Debug.LogWarning("databaseOutputText is NULL!");
        }
    }

    public void showRockAnalysisScreen()
    {
        Debug.Log("Run Analysis Button Clicked");
        StartCoroutine(showAnalysisScreenCoroutine()); // <-- StartCoroutine required
    }

    public void showRockAnalysisScreenNoRun()
    {
        Debug.Log("Back to Show Analysis screen");
        hideAllScreens();
        AnalysisScreenObj.SetActive(true);
    }

    public IEnumerator showAnalysisScreenCoroutine()
    {
        hideAllScreens();

        Debug.Log("Starting Rock Analysis...");

        // Fetch rock data
        var results = analyzer.AnalyzeRock();

        Debug.Log("Finished fetching data from server.");

        yield return new WaitForSeconds(2.0f);

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

        // Create new entry and save it
        int newId = rockAnalysisDatabase.Count > 0 ? rockAnalysisDatabase.Max(entry => entry.id) + 1 : 1;
        RockAnalysisEntry newEntry = new RockAnalysisEntry(newId, results);
        currentEntryBeingEdited = newEntry;

        rockAnalysisDatabase.Add(newEntry);
        SaveDatabase();

        Debug.Log("Finished coroutine completely");
    }


}
