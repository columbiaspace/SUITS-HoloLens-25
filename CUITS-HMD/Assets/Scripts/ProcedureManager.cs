using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using System.Linq; // Not strictly needed for this simplified version

// BackendProcedureStep and ProcedureActionableItem are now defined in BackendDataService.cs
// Ensure BackendDataService.cs is compiled before this script or in an assembly this script references.

public class ProcedureManager : MonoBehaviour
{
    // UI GameObjects for indicators (red stickers)
    public GameObject EMU1_POWER_Indicator;
    public GameObject EV1_SUPPLY_Indicator;
    public GameObject EV1_WASTE_Indicator;
    // public GameObject EV2_SUPPLY_Indicator; // Assuming single EV focus for now based on Egress.cs
    // public GameObject EV2_WASTE_Indicator;  // If procedures are EV specific
    // public GameObject EMU2_POWER_Indicator;
    public GameObject EMU1_OXY_Indicator;
    // public GameObject EMU2_OXY_Indicator;
    public GameObject O2_VENT_Indicator;
    public GameObject DEPRESS_PUMP_Indicator;
    // Add more GameObjects for DCU switch indicators if they are separate visual elements

    public TMP_Text instructionDisplay_Text;

    // Store a mapping from logical item names (from backend) to GameObjects
    private Dictionary<string, GameObject> procedureItemIndicators;

    void Start()
    {
        InitializeIndicators();
        // Initially, hide all indicators and set a default message
        SetAllIndicators(false);
        if (instructionDisplay_Text != null) instructionDisplay_Text.text = "Waiting for procedure data...";
    }

    void InitializeIndicators()
    {
        procedureItemIndicators = new Dictionary<string, GameObject>();
        if (EMU1_POWER_Indicator != null) procedureItemIndicators["EMU1_POWER"] = EMU1_POWER_Indicator;
        if (EV1_SUPPLY_Indicator != null) procedureItemIndicators["EV1_WATER_SUPPLY"] = EV1_SUPPLY_Indicator; // Match potential backend key
        if (EV1_WASTE_Indicator != null) procedureItemIndicators["EV1_WATER_WASTE"] = EV1_WASTE_Indicator;   // Match potential backend key
        if (EMU1_OXY_Indicator != null) procedureItemIndicators["EMU1_OXY"] = EMU1_OXY_Indicator;
        if (O2_VENT_Indicator != null) procedureItemIndicators["O2_VENT"] = O2_VENT_Indicator;
        if (DEPRESS_PUMP_Indicator != null) procedureItemIndicators["DEPRESS_PUMP"] = DEPRESS_PUMP_Indicator;
        // Add mappings for other indicators (EV2, DCU items if they have separate GameObjects)
    }

    void SetAllIndicators(bool isActive)
    {
        foreach (var indicator in procedureItemIndicators.Values)
        {
            if (indicator != null) indicator.SetActive(isActive);
        }
    }

    void Update()
    {
        if (BackendDataService.Instance != null && BackendDataService.Instance.LatestData != null && BackendDataService.Instance.LatestData.current_procedure_step != null)
        {
            BackendProcedureStep currentStep = BackendDataService.Instance.LatestData.current_procedure_step;
            
            if (currentStep.is_completed)
            {
                if (instructionDisplay_Text != null) instructionDisplay_Text.text = currentStep.procedure_name + " Complete!";
                SetAllIndicators(false);
                return;
            }

            if (instructionDisplay_Text != null)
            {
                instructionDisplay_Text.text = $"Step {currentStep.step_number}: {currentStep.instruction_text}";
            }

            // Deactivate all indicators first
            SetAllIndicators(false);

            // Activate indicators for items that need attention (current state does not match target)
            if (currentStep.actionable_items != null)
            {
                foreach (var item in currentStep.actionable_items)
                {
                    if (procedureItemIndicators.TryGetValue(item.item_name, out GameObject indicator))
                    {
                        if (indicator != null)
                        {
                            // Show indicator if the backend says the state doesn't match
                            // (i.e., user needs to flip this switch)
                            indicator.SetActive(!item.current_state_matches);
                        }
                    }
                    else
                    {
                        // This could also be an HMD check (e.g. "Wait until pressure < 10psi")
                        // For these, current_state_matches being false means the condition isn't met yet.
                        // The instruction_text should guide the user.
                         Debug.LogWarning($"No UI indicator found for procedure item: {item.item_name}. This might be an HMD-based check.");
                    }
                }
            }
        }
        else
        {
            // Handle cases where data or specific procedure step might be null
            string statusMessage = "Waiting for data...";
            if (BackendDataService.Instance == null)
            {
                statusMessage = "Backend service not available.";
            }
            else if (BackendDataService.Instance.LatestData == null)
            {
                statusMessage = "Connecting to backend...";
            }
            else if (BackendDataService.Instance.LatestData.current_procedure_step == null)
            {
                 statusMessage = "No active procedure.";
            }
            if (instructionDisplay_Text != null) instructionDisplay_Text.text = statusMessage;
            SetAllIndicators(false);
        }
    }

    // Remove or comment out this simulation function when your backend provides the real data structure.
    /*
    private int demoStep = 0;
    private float demoStepTimer = 0f;
    private float demoStepDuration = 5f; 
    BackendProcedureStep SimulateBackendProcedureData()
    {
        demoStepTimer += Time.deltaTime;
        if (demoStepTimer > demoStepDuration)
        {
            demoStep++;
            demoStepTimer = 0f;
        }

        BackendProcedureStep step = new BackendProcedureStep();
        step.procedure_name = "EVA Egress";
        step.actionable_items = new List<ProcedureActionableItem>();

        switch (demoStep)
        {
            case 0:
                step.step_number = 1;
                step.instruction_text = "Connect UIA to DCU umbilical. Set EV-1 PWR ON.";
                step.actionable_items.Add(new ProcedureActionableItem { item_name = "EMU1_POWER", target_state_description = "ON", current_state_matches = false });
                break;
            case 1:
                step.step_number = 2;
                step.instruction_text = "Set DEPRESS PUMP PWR ON.";
                step.actionable_items.Add(new ProcedureActionableItem { item_name = "DEPRESS_PUMP", target_state_description = "ON", current_state_matches = false });
                break;
            case 2:
                step.step_number = 3; 
                step.instruction_text = "Open OXYGEN O2 VENT.";
                step.actionable_items.Add(new ProcedureActionableItem { item_name = "O2_VENT", target_state_description = "OPEN", current_state_matches = false });
                break;
            case 3:
                step.step_number = 4; 
                step.instruction_text = "Wait until EV1 Primary and Secondary OXY tanks are < 10psi.";
                step.actionable_items.Add(new ProcedureActionableItem { item_name = "HMD_OXY_TANKS_LOW_CHECK", target_state_description = "<10psi", current_state_matches = false });
                break;
            default:
                step.step_number = 99;
                step.instruction_text = "All simulated steps complete.";
                step.is_completed = true;
                demoStep = -1; 
                break;
        }
        return step;
    }
    */
} 