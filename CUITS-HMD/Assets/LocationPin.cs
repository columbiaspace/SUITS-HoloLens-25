using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class IMUResponse
{
    public DCU dcu;
    
    [Serializable]
    public class DCU
    {
        public EVAData eva1;
        
        [Serializable]
        public class EVAData
        {
            public double posx;
            public double posy;
            public double heading;
        }
    }
}

public class LocationPin : MonoBehaviour
{
    [Header("Backend Settings")]
    public string backendUrl = "http://localhost:8000"; // Base URL of your backend API
    public float updateInterval = 0.5f;                 // How often to fetch position data (seconds)

    [Header("References")]
    public GameObject mapObject;            // Reference to the map game object
    public GameObject pinPrefab;            // The pin GameObject/prefab
    
    [Header("Position Settings")]
    public float pinHeight = 0.01f;         // How high above the map to place the pin
    public Vector2 positionScaleFactor = new Vector2(1.0f, 1.0f);  // Scale factor for coordinates
    public Vector2 positionOffset = new Vector2(0.0f, 0.0f);       // Offset for positioning
    public bool rotateWithHeading = true;   // Whether the pin should rotate with user heading

    private GameObject activePinObject;     // Reference to the instantiated pin
    private Renderer mapRenderer;           // Map's renderer component
    private float nextUpdate = 0.0f;
    private Coroutine updateCoroutine;      // Reference to the coroutine for clean up

    void Start()
    {
        // Find map object if not assigned
        if (mapObject == null)
        {
            mapObject = GameObject.Find("Map");
            if (mapObject == null)
            {
                Debug.LogError("LocationPin: No Map object found! Please assign the map object in the inspector.");
                return;
            }
        }
        
        // Get the renderer for the map
        mapRenderer = mapObject.GetComponent<Renderer>();
        if (mapRenderer == null)
        {
            Debug.LogError("LocationPin: Map object doesn't have a Renderer component!");
            return;
        }

        // Create pin if we have a prefab
        if (pinPrefab != null)
        {
            activePinObject = Instantiate(pinPrefab, Vector3.zero, Quaternion.identity);
            activePinObject.transform.parent = mapObject.transform; // Make pin a child of the map
        }
        else
        {
            // Create a simple pin as a fallback if no prefab is provided
            activePinObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            activePinObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            activePinObject.transform.parent = mapObject.transform;
            
            // Add a material with a bright color
            Renderer pinRenderer = activePinObject.GetComponent<Renderer>();
            Material pinMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            pinMaterial.color = Color.red;
            pinRenderer.material = pinMaterial;
        }

        // Set initial position at map center
        activePinObject.transform.position = new Vector3(
            mapObject.transform.position.x,
            mapObject.transform.position.y + pinHeight,
            mapObject.transform.position.z
        );
        
        // Start the position update coroutine
        updateCoroutine = StartCoroutine(FetchPositionDataLoop());
    }
    
    void OnDestroy()
    {
        // Clean up the coroutine when the object is destroyed
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    IEnumerator FetchPositionDataLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FetchEVA1Position());
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    IEnumerator FetchEVA1Position()
    {
        string url = $"{backendUrl}/eva1/imu";
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Set request timeout
            webRequest.timeout = 5; // 5 second timeout
            
            // Send the request
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching EVA1 position: {webRequest.error}");
            }
            else
            {
                try
                {
                    // Parse the JSON response
                    string jsonResponse = webRequest.downloadHandler.text;
                    IMUResponse data = JsonUtility.FromJson<IMUResponse>(jsonResponse);
                    
                    // Update pin position with the received data
                    if (data != null && data.dcu != null && data.dcu.eva1 != null)
                    {
                        UpdatePinPosition(data.dcu.eva1.posx, data.dcu.eva1.posy, data.dcu.eva1.heading);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse EVA1 position data: {e.Message}");
                }
            }
        }
    }

    void UpdatePinPosition(double posX, double posY, double heading)
    {
        if (activePinObject == null || mapObject == null) return;
        
        // Convert to map coordinates
        Vector3 mapPos = ConvertTSSPositionToMapPosition(posX, posY);
        
        // Update pin position
        activePinObject.transform.position = mapPos;
        
        // Update rotation based on heading if enabled
        if (rotateWithHeading)
        {
            // Convert heading to Unity rotation (adjust as needed for your map orientation)
            float rotation = (float)heading;
            activePinObject.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
        
        // Log position for debugging
        Debug.Log($"Updated pin position: {mapPos}, heading: {heading}");
    }

    Vector3 ConvertTSSPositionToMapPosition(double tssPosX, double tssPosY)
    {
        // Get map bounds
        Bounds mapBounds = mapRenderer.bounds;
        
        // Convert from TSS position coordinates to relative position (0-1)
        float normalizedX = (float)tssPosX * positionScaleFactor.x + positionOffset.x;
        float normalizedZ = (float)tssPosY * positionScaleFactor.y + positionOffset.y;
        
        // Map to the physical map's bounds
        float x = mapBounds.min.x + normalizedX * mapBounds.size.x;
        float z = mapBounds.min.z + normalizedZ * mapBounds.size.z;
        
        // Use the map's Y position plus a small offset for the pin height
        float y = mapObject.transform.position.y + pinHeight;
        
        return new Vector3(x, y, z);
    }
    
    // Public method to force a position update
    public void ForceUpdate()
    {
        StartCoroutine(FetchEVA1Position());
    }
}