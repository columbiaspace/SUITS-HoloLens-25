using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropPin : MonoBehaviour
{
    public GameObject[] pins; // Array to store all the objects (pins) you want to appear
    public GameObject ev1PinPrefab; // Prefab for EV1 pin
    public GameObject ev2PinPrefab; // Prefab for EV2 pin
    public GameObject pinLabelPrefab; // Optional prefab for text labels
    
    [Header("Pin Settings")]
    public int maxPins = 10; // Maximum number of pins to display
    public bool showBackendPins = true; // Display pins from backend
    public bool trackEVAPositions = true; // Track EV1/EV2 positions
    public int evaToTrack = 1; // Which EVA to track (1 or 2)
    
    // Scale and offset values from the original APIClient conversion
    private const float SCALE_FACTOR = 0.162f;
    private const float OFFSET_X = 916.52f;
    private const float OFFSET_Y = 1619.58f;
    private const float Z_POS = -0.5f;
    
    private Vector3 latestPosition; // Latest position for button-based pin placement
    private int pressCount = 0; // Counter for button presses
    private List<GameObject> dynamicPins = new List<GameObject>(); // For pins created at runtime
    private Dictionary<string, GameObject> backendPinObjects = new Dictionary<string, GameObject>(); // Map backend pin IDs to GameObjects

    // Legacy support - Set positions dynamically (used in legacy APIClient)
    public void SetPosition(Vector3 newPosition)
    {
        latestPosition = newPosition;
        Debug.Log($"DropPin: Position set externally to {newPosition}");
    }

    public void ButtonPress()
    {
        if (pins != null && pressCount < pins.Length)
        {
            // Use existing pin GameObjects from the inspector array
            pins[pressCount].SetActive(true);
            pins[pressCount].transform.localPosition = latestPosition;
            Debug.Log($"DropPin: Placed pin {pressCount} at {latestPosition}");
            pressCount++;
        }
        else
        {
            // Create a new pin dynamically if we've used all predefined pins
            CreatePin(latestPosition, $"Pin {dynamicPins.Count + 1}");
        }
        
        // If BackendDataService is available, we could also send this pin to the backend
        if (BackendDataService.Instance != null)
        {
            Debug.Log("DropPin: Consider implementing pin creation on backend via API call");
        }
    }

    void CreatePin(Vector3 position, string label)
    {
        if (dynamicPins.Count >= maxPins)
        {
            // Recycle oldest pin if we hit the limit
            GameObject oldestPin = dynamicPins[0];
            dynamicPins.RemoveAt(0);
            Destroy(oldestPin);
        }
        
        // Create new pin
        GameObject pinPrefab = ev1PinPrefab != null ? ev1PinPrefab : pins.Length > 0 ? pins[0] : null;
        if (pinPrefab != null)
        {
            GameObject newPin = Instantiate(pinPrefab, transform);
            newPin.transform.localPosition = position;
            newPin.SetActive(true);
            dynamicPins.Add(newPin);
            
            // Add label if prefab exists
            if (pinLabelPrefab != null && !string.IsNullOrEmpty(label))
            {
                GameObject labelObj = Instantiate(pinLabelPrefab, newPin.transform);
                TMP_Text labelComponent = labelObj.GetComponent<TMP_Text>();
                if (labelComponent != null)
                {
                    labelComponent.text = label;
                }
            }
            
            Debug.Log($"DropPin: Created dynamic pin at {position} with label '{label}'");
        }
        else
        {
            Debug.LogError("DropPin: Cannot create pin - no prefab available!");
        }
    }

    void Start()
    {
        // Initially hide all pins
        if (pins != null)
        {
            foreach (var pin in pins)
            {
                if (pin != null)
                {
                    pin.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("DropPin: Null pin found in pins array!");
                }
            }
        }
        
        Debug.Log("DropPin: Started. Ready to place pins.");
    }
    
    void Update()
    {
        if (BackendDataService.Instance != null && BackendDataService.Instance.LatestData != null)
        {
            // Update EV positions
            if (trackEVAPositions)
            {
                // Update latestPosition based on which EVA we're tracking
                if (evaToTrack == 1 && BackendDataService.Instance.LatestData.eva1?.imu != null)
                {
                    UpdateEVAPosition(BackendDataService.Instance.LatestData.eva1.imu, 1);
                }
                else if (evaToTrack == 2 && BackendDataService.Instance.LatestData.eva2?.imu != null)
                {
                    UpdateEVAPosition(BackendDataService.Instance.LatestData.eva2.imu, 2);
                }
            }
            
            // Update backend pins
            if (showBackendPins && BackendDataService.Instance.LatestData.pins != null)
            {
                UpdateBackendPins(BackendDataService.Instance.LatestData.pins);
            }
        }
    }
    
    void UpdateEVAPosition(IMUData imuData, int evaNum)
    {
        // Convert coordinates using the same formula as in the original APIClient
        Vector3 position = new Vector3(
            SCALE_FACTOR * imuData.posx + OFFSET_X,
            SCALE_FACTOR * imuData.posy + OFFSET_Y,
            Z_POS
        );
        
        // Update latestPosition for button-based pin placement
        latestPosition = position;
        
        // For automatic tracking, you could update a dedicated EVA pin here
        // This would be a pin that follows the EVA position constantly
        string evaKey = $"EVA{evaNum}";
        if (!backendPinObjects.ContainsKey(evaKey))
        {
            // Create a special pin for this EVA if it doesn't exist
            GameObject pinPrefab = evaNum == 1 ? ev1PinPrefab : ev2PinPrefab;
            if (pinPrefab != null)
            {
                GameObject evaPin = Instantiate(pinPrefab, transform);
                evaPin.name = $"EVA{evaNum}_PositionPin";
                backendPinObjects[evaKey] = evaPin;
            }
        }
        
        // Update the EVA pin position
        if (backendPinObjects.TryGetValue(evaKey, out GameObject evaPinObj))
        {
            evaPinObj.transform.localPosition = position;
        }
    }
    
    void UpdateBackendPins(List<PinData> pins)
    {
        if (pins == null) return;
        
        // Track which pins we've seen to clean up old ones
        HashSet<string> currentPinIds = new HashSet<string>();
        
        foreach (var pin in pins)
        {
            // Create unique ID for this pin based on its data
            string pinId = $"pin_{pin.ev}_{pin.posx}_{pin.posy}";
            currentPinIds.Add(pinId);
            
            if (!backendPinObjects.ContainsKey(pinId))
            {
                // Convert coordinates using same formula
                Vector3 position = new Vector3(
                    SCALE_FACTOR * pin.posx + OFFSET_X,
                    SCALE_FACTOR * pin.posy + OFFSET_Y,
                    Z_POS
                );
                
                // Create pin based on EV number
                GameObject pinPrefab = pin.ev == 1 ? ev1PinPrefab : ev2PinPrefab;
                if (pinPrefab != null)
                {
                    GameObject newPin = Instantiate(pinPrefab, transform);
                    newPin.transform.localPosition = position;
                    newPin.name = !string.IsNullOrEmpty(pin.label) ? pin.label : $"Pin_{pin.ev}";
                    
                    // Add label if needed
                    if (pinLabelPrefab != null && !string.IsNullOrEmpty(pin.label))
                    {
                        GameObject labelObj = Instantiate(pinLabelPrefab, newPin.transform);
                        TMP_Text labelComponent = labelObj.GetComponent<TMP_Text>();
                        if (labelComponent != null)
                        {
                            labelComponent.text = pin.label;
                        }
                    }
                    
                    backendPinObjects[pinId] = newPin;
                }
            }
            else
            {
                // Pin already exists, update position if needed
                GameObject existingPin = backendPinObjects[pinId];
                Vector3 newPosition = new Vector3(
                    SCALE_FACTOR * pin.posx + OFFSET_X,
                    SCALE_FACTOR * pin.posy + OFFSET_Y,
                    Z_POS
                );
                
                // Only update if position changed
                if (Vector3.Distance(existingPin.transform.localPosition, newPosition) > 0.1f)
                {
                    existingPin.transform.localPosition = newPosition;
                }
            }
        }
        
        // Clean up pins that are no longer in the data
        List<string> pinKeysToRemove = new List<string>();
        foreach (var kvp in backendPinObjects)
        {
            // Skip EVA tracking pins which are managed separately
            if (kvp.Key == "EVA1" || kvp.Key == "EVA2") continue;
            
            if (!currentPinIds.Contains(kvp.Key))
            {
                // This pin is no longer in the backend data
                pinKeysToRemove.Add(kvp.Key);
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }
        }
        
        // Remove deleted pins from dictionary
        foreach (var key in pinKeysToRemove)
        {
            backendPinObjects.Remove(key);
        }
    }
}
