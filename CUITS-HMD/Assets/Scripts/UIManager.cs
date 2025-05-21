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
    public GameObject RecordViewObj;
    public GameObject RecordCardPrefab; // Assign RecordCard prefab
    public Transform cardListParent; // Assign ScrollView Content area

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
        RecordViewObj.SetActive(false);
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

    public void showDatabaseScreen(){
        hideAllScreens();
        ViewDatabaseObj.SetActive(true);
        // Clear old cards
        foreach (Transform child in cardListParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < rockAnalysisDatabase.Count; i++)
        {
            var entry = rockAnalysisDatabase[i];
            GameObject card = Instantiate(RecordCardPrefab, cardListParent);

            // Set the size manually
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800f, 300f); // 👈 Adjust width/height here

            // TextMeshProUGUI timestampText = card.GetComponentInChildren<TextMeshProUGUI>();
            // timestampText.text = "rock_id " + entry.id + " at " + entry.timestamp;

            TextMeshProUGUI cardText = card.GetComponentInChildren<TextMeshProUGUI>();

            string mostAbundantChemical = "N/A";
            float maxValue = float.MinValue;

            foreach (var kvp in entry.chemicalData)
            {
                if (kvp.Value > maxValue)
                {
                    maxValue = kvp.Value;
                    mostAbundantChemical = kvp.Key;
                }
            }

            cardText.text = $"rock_id {entry.id} at {entry.timestamp}\nMost Abundant: {mostAbundantChemical} ({maxValue})";
            // timestampText.fontSize = 36f; // Set font size here

            int index = i; // local copy for lambda
            card.GetComponent<Button>().onClick.AddListener(() => ShowRecordDetails(index));

        }
    }

    public void ShowRecordDetails(int index)
    {
        hideAllScreens();
        RecordViewObj.SetActive(true);

        if (index < 0 || index >= rockAnalysisDatabase.Count)
        {
            Debug.LogWarning("Invalid record index.");
            return;
        }
        var entry = rockAnalysisDatabase[index];
        string output = $"Analysis ID: {entry.id}\n";
        output += $"Timestamp: {entry.timestamp}\n\n";

        foreach (var kvp in entry.chemicalData)
        {
            output += $"{kvp.Key}: {kvp.Value}\n";
        }

        output += $"\nColor: {entry.color}\nSize: {entry.size}\nTexture: {entry.texture}\nOther Notes: {entry.otherNotes}";

        databaseOutputText.text = output;
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

    // public IEnumerator showAnalysisScreenCoroutine()
    // {
    //     hideAllScreens();

    //     Debug.Log("Starting Rock Analysis...");

    //     yield return new WaitForSeconds(1.0f); // Simulate slight delay

        
    //     var backend = BackendDataService.Instance;

    //     if (backend == null || backend.LatestData == null || backend.LatestData.eva1 == null || backend.LatestData.eva1.spec == null)
    //     {
    //         Debug.LogWarning("Spec data not available from backend.");
    //         analysisOutputText.text = "No rock data available.";
    //         yield break;
    //     }

    //     var spec = backend.LatestData.eva1.spec;


    //     var results = new Dictionary<string, float>
    //     {
    //         { "SiO2", (float)spec.SiO2 },
    //         { "TiO2", (float)spec.TiO2 },
    //         { "Al2O3", (float)spec.Al2O3 },
    //         { "FeO",  (float)spec.FeO },
    //         { "MnO",  (float)spec.MnO },
    //         { "MgO",  (float)spec.MgO },
    //         { "CaO",  (float)spec.CaO },
    //         { "K2O",  (float)spec.K2O },
    //         { "P2O3", (float)spec.P2O3 },
    //         { "Other",(float)spec.other }
    //     };

    //     Debug.Log("Received SpecData from backend:");
    //     foreach (var kvp in results)
    //     {
    //         Debug.Log($"{kvp.Key}: {kvp.Value}");
    //     }

    //     // Build output string for UI
    //     string output = "EVA1 chemical composition:\n";
    //     foreach (var kvp in results)
    //     {
    //         output += $"{kvp.Key}: {kvp.Value}\n";
    //     }

    //     // Update UI
    //     analysisOutputText.text = output;
    //     AnalysisScreenObj.SetActive(true);

    //     // Prepare entry, don't save yet
    //     int newId = rockAnalysisDatabase.Count > 0 ? rockAnalysisDatabase.Max(entry => entry.id) + 1 : 1;
    //     RockAnalysisEntry newEntry = new RockAnalysisEntry(newId, results)
    //     {
    //         name = spec.name // If you want to capture the rock name
    //     };

    //     currentEntryBeingEdited = newEntry;

    //     Debug.Log("Analysis ready. Awaiting Save Record button.");
    // }

    public IEnumerator showAnalysisScreenCoroutine()
    {
        hideAllScreens();

        Debug.Log("Starting Rock Analysis...");

        var results = analyzer.AnalyzeRock();
        Debug.Log("Finished fetching data from server.");

        Debug.Log("Received chemical composition from server:");
        foreach (var kvp in results)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }

        yield return new WaitForSeconds(1.0f);

        string output = "EVA1 chemical composition:\n";
        foreach (var kvp in results)
        {
            output += $"{kvp.Key}: {kvp.Value}\n";
        }

        analysisOutputText.text = output;
        AnalysisScreenObj.SetActive(true);

        // Only create the entry, don't add to database yet
        int newId = rockAnalysisDatabase.Count > 0 ? rockAnalysisDatabase.Max(entry => entry.id) + 1 : 1;
        RockAnalysisEntry newEntry = new RockAnalysisEntry(newId, results);
        currentEntryBeingEdited = newEntry;

        Debug.Log("Analysis ready, waiting for user to save record.");
    }



    public void SaveCurrentRecord()
    {
        if (currentEntryBeingEdited != null)
        {
            rockAnalysisDatabase.Add(currentEntryBeingEdited);
            SaveDatabase();
            Debug.Log("Current rock analysis saved to database.");
        }
        else
        {
            Debug.LogWarning("No record to save.");
        }
    }



}
