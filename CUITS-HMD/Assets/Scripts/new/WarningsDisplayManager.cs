using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text; // For StringBuilder

public class WarningsDisplayManager : MonoBehaviour
{
    public TMP_Text warningsDisplayText; // Assign your TextMeshProUGUI element here
    public int maxWarningsToShow = 5; // Max number of warnings to display at once

    private List<WarningData> currentWarnings = new List<WarningData>();

    void Start()
    {
        if (warningsDisplayText == null)
        {
            Debug.LogError("WarningsDisplayText is not assigned in WarningsDisplayManager!");
            enabled = false; // Disable script if no display text is assigned
            return;
        }
        warningsDisplayText.text = "No active warnings.";
    }

    void Update()
    {
        if (BackendDataService.Instance != null && 
            BackendDataService.Instance.LatestData != null && 
            BackendDataService.Instance.LatestData.warnings != null)
        {
            List<WarningData> newWarnings = BackendDataService.Instance.LatestData.warnings;
            
            // Basic check to see if warnings list has changed to avoid unnecessary UI updates
            // A more robust check would compare content if performance becomes an issue.
            if (!AreWarningsSame(currentWarnings, newWarnings))
            {
                currentWarnings = new List<WarningData>(newWarnings); // Update our copy
                UpdateWarningsUI();
            }
        }
        else
        {
            if (warningsDisplayText.text != "Waiting for warning data...")
            {
                warningsDisplayText.text = "Waiting for warning data...";
                currentWarnings.Clear();
            }
        }
    }

    bool AreWarningsSame(List<WarningData> oldWarnings, List<WarningData> newWarnings)
    {
        if (oldWarnings.Count != newWarnings.Count) return false;
        // This is a shallow check. For true sameness, you'd compare IDs or content.
        // For now, if count is same, we assume if backend sends new list, it might be different.
        // This is just to prevent updating text every frame if the list reference changes but content doesn't.
        // A simple approach for now: if the references are different, assume different.
        // For a more robust solution, you'd iterate and compare specific fields.
        return oldWarnings == newWarnings; // This only checks reference, might not be enough
    }

    void UpdateWarningsUI()
    {
        if (warningsDisplayText == null) return;

        if (currentWarnings.Count == 0)
        {
            warningsDisplayText.text = "No active warnings.";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<color=#FFA500><b>Active Warnings:</b></color>"); // Orange color for header

        int count = 0;
        // Display warnings, respecting maxWarningsToShow. Could sort by severity first.
        // For now, just taking the first few.
        foreach (WarningData warning in currentWarnings)
        {
            if (count >= maxWarningsToShow) 
            {
                sb.AppendLine($"...and {currentWarnings.Count - maxWarningsToShow} more.");
                break;
            }

            string severityColor = "white";
            switch (warning.severity)
            {
                case 1: // Low
                    severityColor = "green"; 
                    break;
                case 2: // Medium
                    severityColor = "yellow";
                    break;
                case 3: // High
                    severityColor = "red";
                    break;
            }
            sb.AppendLine($"- <color={severityColor}>[{warning.source}] {warning.message}</color>");
            count++;
        }
        warningsDisplayText.text = sb.ToString();
    }
} 