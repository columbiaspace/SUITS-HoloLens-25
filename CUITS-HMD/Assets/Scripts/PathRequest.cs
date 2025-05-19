using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PathRequest : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer; // Reference to be set in Inspector
    private string apiUrl = "http://127.0.0.1:8000/compute_path";

    // Start and end points for testing
    private Vector2Int start = new Vector2Int(1, 7);
    private Vector2Int end = new Vector2Int(9, 4);

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer not assigned! Please assign it in the Inspector.");
            return;
        }
        // Start the coroutine to get the path
        StartCoroutine(GetPath(start, end));
    }

    IEnumerator GetPath(Vector2Int start, Vector2Int end)
    {
        // Create the JSON payload
        string jsonData = "{\"start_point\": [" + start.x + ", " + start.y + "], \"end_point\": [" + end.x + ", " + end.y + "]}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Setup the request
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Path data received: " + request.downloadHandler.text);
            ProcessPath(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error fetching path: " + request.error);
        }
    }

    void ProcessPath(string jsonResponse)
    {
        try
        {
            // Parse the response
            PathData pathData = JsonUtility.FromJson<PathData>(jsonResponse);

            if (pathData.path != null)
            {
                Debug.Log("Path length: " + pathData.path.Length);
                foreach (var point in pathData.path)
                {
                    Debug.Log($"Path Point: ({point[0]}, {point[1]})");
                }
                DrawPath(pathData.path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to process path: " + e.Message);
        }
    }

    void DrawPath(int[][] path)
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
            return;
        }

        lineRenderer.positionCount = path.Length;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        for (int i = 0; i < path.Length; i++)
        {
            Vector3 position = new Vector3(path[i][1], 0, path[i][0]);
            lineRenderer.SetPosition(i, position);
        }

        Debug.Log("Path plotted on the map.");
    }

    [System.Serializable]
    private class PathData
    {
        public int[][] path;
    }
}
