using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic; // Required for List
using Newtonsoft.Json; // Required for JsonConvert

public class BackendConnector : MonoBehaviour
{
    // --- Public Fields (Inspectable in Editor) ---
    [Tooltip("The IP address or hostname of the machine running the SUITS-Backend.")]
    public string backendHost = "127.0.0.1"; // Default to localhost

    [Tooltip("The port the SUITS-Backend is running on.")]
    public int backendPort = 8000; // Default FastAPI port

    [Tooltip("How often (in seconds) to request updated data from the backend.")]
    public float updateInterval = 1.0f; // Update every second

    // --- Public Properties (Read-only access to data) ---
    public AllTssData LatestData { get; private set; } // Stores the most recent data
    public bool IsConnected { get; private set; } // Indicates if the first successful connection was made
    public string LastError { get; private set; } // Stores the last error message
    public bool IsRequestPending { get; private set; } // True while waiting for a response

    // --- Private Fields ---
    private string backendUrl;
    private float timeSinceLastUpdate = 0f;

    // --- Unity Methods ---
    void Start()
    {
        backendUrl = $"http://{backendHost}:{backendPort}/all";
        Debug.Log($"BackendConnector targeting: {backendUrl}");
        LatestData = new AllTssData(); // Initialize with empty data
        IsConnected = false;
        LastError = null;
        IsRequestPending = false;

        // Start the update loop
        StartCoroutine(FetchDataRoutine());
    }

    // --- Coroutines ---
    private IEnumerator FetchDataRoutine()
    {
        while (true) // Loop indefinitely
        {
            if (!IsRequestPending)
            {
                yield return StartCoroutine(RequestBackendData());
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private IEnumerator RequestBackendData()
    {
        IsRequestPending = true;
        LastError = null; // Clear previous error

        using (UnityWebRequest webRequest = UnityWebRequest.Get(backendUrl))
        {
            // Set a reasonable timeout
            webRequest.timeout = (int)(updateInterval * 0.8f); // Timeout slightly less than interval

            // Send the request and wait for a response or timeout
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    LastError = $"Connection Error: {webRequest.error}";
                    Debug.LogError($"BackendConnector: {LastError}");
                    IsConnected = false; // Assume disconnected on error
                    break;

                case UnityWebRequest.Result.ProtocolError: // HTTP error e.g., 404, 500
                    LastError = $"HTTP Error: {webRequest.responseCode} - {webRequest.error}";
                    Debug.LogError($"BackendConnector: {LastError} - Response: {webRequest.downloadHandler.text}");
                    IsConnected = false;
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    LastError = $"Data Processing Error: {webRequest.error}";
                    Debug.LogError($"BackendConnector: {LastError}");
                    IsConnected = false;
                    break;

                case UnityWebRequest.Result.Success:
                    try
                    {
                        string jsonResponse = webRequest.downloadHandler.text;
                        // Use Newtonsoft.Json for deserialization
                        LatestData = JsonConvert.DeserializeObject<AllTssData>(jsonResponse);
                        IsConnected = true; // Mark as connected after first success
                        // Optional: Log successful data fetch
                        // Debug.Log("BackendConnector: Data updated successfully.");
                    }
                    catch (JsonException jsonEx)
                    {
                        LastError = $"JSON Parsing Error: {jsonEx.Message}";
                        Debug.LogError($"BackendConnector: {LastError}\nReceived JSON:\n{webRequest.downloadHandler.text}");
                        IsConnected = false; // Data is invalid
                    }
                    catch (System.Exception ex)
                    {
                        LastError = $"Error processing data: {ex.Message}";
                        Debug.LogError($"BackendConnector: {LastError}");
                        IsConnected = false;
                    }
                    break;
            }
        }
        IsRequestPending = false;
    }
}
