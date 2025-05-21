using UnityEngine;
using TMPro;
using System.Collections.Generic;
// using UnityEngine.UI; // No longer required

public class ProcedureBlockViewer : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text procedureBlockDisplayText;
    public TMP_Text procedureTitleText; // To show the current block's title
    // public Button nextButton; // Removed
    // public Button prevButton; // Removed

    private List<ProcedureSection> procedureSections;
    private int currentSectionIndex = 0;

    private struct ProcedureSection
    {
        public string title;
        public string content;
    }

    void Start()
    {
        InitializeProcedureSections();
        UpdateUI();

        // Listeners for standard UI Buttons removed, MRTK PressableButton events will be set in Inspector
        // if (nextButton != null) nextButton.onClick.AddListener(NextSection);
        // if (prevButton != null) prevButton.onClick.AddListener(PreviousSection);
    }

    void InitializeProcedureSections()
    {
        procedureSections = new List<ProcedureSection>();

        procedureSections.Add(new ProcedureSection {
            title = "EVA Egress",
            content = "1. **Verify LTV Coordination**\n" +
                      "   1. Verify ping from LTV\n" +
                      "   2. Verify worksite POIs provided\n" +
                      "   3. Verify EV1 has received POIs\n" +
                      "   4. Announce PR operations complete; handover to EVA\n" +
                      "2. **Connect UIA → DCU & Start Depress**\n" +
                      "   1. EV1 verify umbilical UIA→DCU\n" +
                      "   2. UIA: EMU PWR – ON\n" +
                      "   3. DCU: BATT – UMB\n" +
                      "   4. UIA: DEPRESS PUMP PWR – ON\n" +
                      "3. **Prep O₂ Tanks**\n" +
                      "   1. UIA: O₂ VENT – OPEN\n" +
                      "   2. Wait until both O₂ tanks < 10 psi\n" +
                      "   3. UIA: O₂ VENT – CLOSE\n" +
                      "   4. DCU: OXY – PRI\n" +
                      "   5. UIA: EMU-1 OXY – OPEN\n" +
                      "   6. Wait until primary tank > 3000 psi\n" +
                      "   7. UIA: EMU-1 OXY – CLOSE\n" +
                      "   8. DCU: OXY – SEC\n" +
                      "   9. UIA: EMU-1 OXY – OPEN\n" +
                      "   10. Wait until secondary tank > 3000 psi\n" +
                      "   11. UIA: EMU-1 OXY – CLOSE\n" +
                      "   12. DCU: OXY – PRI\n" +
                      "4. **End Depress, Check Switches & Disconnect**\n" +
                      "   1. Wait until suit pressure & O₂ = 4 psi\n" +
                      "   2. UIA: DEPRESS PUMP PWR – OFF\n" +
                      "   3. DCU: BATT – LOCAL\n" +
                      "   4. UIA: EMU PWR – OFF\n" +
                      "   5. DCU: Verify OXY – PRI; COMMS – A; FAN – PRI; PUMP – CLOSE; CO₂ – A\n" +
                      "   6. EV1 disconnect umbilical UIA↔DCU\n" +
                      "   7. DCU: Comms check (“EV1 to PR, can you hear me?”)"
        });

        procedureSections.Add(new ProcedureSection {
            title = "Navigation",
            content = "1. EV1 drop pins & determine best path for each POI\n" +
                      "2. Verify path generated; await PR “go”\n" +
                      "3. PR unlock airlock; announce all clear\n" +
                      "4. EV1 exit airlock & navigate to worksite"
        });

        procedureSections.Add(new ProcedureSection {
            title = "Geologic Sampling",
            content = "1. Announce arrival: “Arrived at site, beginning sampling.”\n" +
                      "2. **Sampling Procedure**\n" +
                      "   1. HMD: Open sampling procedure\n" +
                      "   2. HMD: Field notes (pictures, voice notes)\n" +
                      "   3. HMD: Perform XRF scan\n" +
                      "      1. XRF: Press & hold trigger\n" +
                      "      2. Aim until beep, then release\n" +
                      "   4. Announce “Scan complete, PR verify data.”\n" +
                      "   5. If composition off-nominal, collect rock\n" +
                      "   6. If able, drop & label pin\n" +
                      "   7. Repeat until area complete\n" +
                      "3. **Significance Criteria** _(for SUITS)_\n" +
                      "   - SiO₂ < 30%\n" +
                      "   - TiO₂ > 10%\n" +
                      "   - Al₂O₃ > 25%\n" +
                      "   - FeO > 20%\n" +
                      "   - MnO > 0.5%\n" +
                      "   - MgO > 10%\n" +
                      "   - CaO < 5%\n" +
                      "   - K₂O > 1%\n" +
                      "   - P₂O₃ > 1%"
        });

        procedureSections.Add(new ProcedureSection {
            title = "Return to Pressurized Rover",
            content = "1. HMD: Verify path to PR\n" +
                      "2. HMD: Begin return to PR"
        });

        procedureSections.Add(new ProcedureSection {
            title = "EVA Ingress",
            content = "1. **Connect UIA → DCU**\n" +
                      "   1. EV1 connect umbilical UIA→DCU\n" +
                      "   2. UIA: EMU PWR – ON\n" +
                      "   3. DCU: BATT – UMB\n" +
                      "2. **Vent O₂ Tanks**\n" +
                      "   1. UIA: O₂ VENT – OPEN\n" +
                      "   2. Wait until both tanks < 10 psi\n" +
                      "   3. UIA: O₂ VENT – CLOSE\n" +
                      "3. **Empty Water Tanks**\n" +
                      "   1. DCU: PUMP – OPEN\n" +
                      "   2. UIA: WASTE WATER – OPEN\n" +
                      "   3. Wait until coolant tank < 5%\n" +
                      "   4. UIA: WASTE WATER – CLOSE\n" +
                      "4. **Disconnect UIA from DCU**\n" +
                      "   1. UIA: EMU PWR – OFF\n" +
                      "   2. DCU: Disconnect umbilical"
        });
    }

    public void NextSection()
    {
        if (currentSectionIndex < procedureSections.Count - 1)
        {
            currentSectionIndex++;
            UpdateUI();
        }
    }

    public void PreviousSection()
    {
        if (currentSectionIndex > 0)
        {
            currentSectionIndex--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (procedureSections == null || procedureSections.Count == 0) return;

        ProcedureSection currentSection = procedureSections[currentSectionIndex];

        if (procedureTitleText != null)
        {
            procedureTitleText.text = currentSection.title;
        }

        if (procedureBlockDisplayText != null)
        {
            procedureBlockDisplayText.text = currentSection.content.Replace("\\n", "\n"); // Ensure newlines are rendered
        }

        // interactable property of standard UI Buttons no longer managed here
        // if (prevButton != null)
        // {
        //     prevButton.interactable = currentSectionIndex > 0;
        // }
        // if (nextButton != null)
        // {
        //     nextButton.interactable = currentSectionIndex < procedureSections.Count - 1;
        // }
    }
} 