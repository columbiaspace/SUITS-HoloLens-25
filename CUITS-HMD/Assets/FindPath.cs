using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FindPath : MonoBehaviour
{
    // Public fields to be assigned in Inspector
    public int poiIndex = 0; // Index for the pois list
    public PathDrawer pathDrawer; // Assign in Unity Inspector

    // Internal state
    private Vector2Int startNode;
    private List<Vector2Int> pois = new List<Vector2Int> {
        new Vector2Int(10, 3), // Assuming these are (x,y) coordinates
        new Vector2Int(12, 3),
        new Vector2Int(12, 6)
    };

    // Added Awake method to verify script initialization
    void Awake()
    {
        Debug.Log("========== FindPath: AWAKE CALLED ==========");
    }

    // This public method can be called when a button is clicked
    public void FindPathOnButtonClick()
    {
        Debug.Log("========== FindPath: BUTTON CLICKED ==========");
        
        if (pathDrawer == null)
        {
            Debug.LogError("FindPath: PathDrawer not assigned in FindPath script.");
            return;
        }
        
        // Build adjacency list if it hasn't been built yet
        if (adjacencyList == null || adjacencyList.Count == 0)
        {
            Debug.Log("FindPath: Building adjacency list...");
            adjacencyList = BuildAdjacencyList(obstacleMap);
            Debug.Log($"FindPath: Adjacency list built with {adjacencyList.Count} nodes");
        }
        
        // Start the process to fetch EVA position and find path
        StartCoroutine(FetchEvaPositionAndProcessPath());
    }

    // Obstacle map (0 = traversable, 1 = obstacle)
    private static readonly List<List<int>> obstacleMap = new List<List<int>>
    {
        new List<int> {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1},
        new List<int> {1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1},
        new List<int> {1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1}
    };

    private Dictionary<Vector2Int, List<Vector2Int>> adjacencyList;

    // Helper class for parsing EVA position from TSS
    // JsonUtility typically requires [System.Serializable]
    [System.Serializable]
    private class EvaPositionResponse
    {
        public float posx;
        public float posy;
        // public float heading; // Include if needed
    }

    void Start()
    {
        Debug.Log("========== FindPath: START METHOD CALLED ==========");
        
        if (pathDrawer == null)
        {
            Debug.LogError("FindPath: PathDrawer not assigned in FindPath script.");
            return;
        }
        if (pois == null || pois.Count == 0)
        {
            Debug.LogError("FindPath: POIs list is not initialized or is empty.");
            return;
        }
        if (poiIndex < 0 || poiIndex >= pois.Count)
        {
            Debug.LogWarning($"FindPath: poiIndex {poiIndex} is out of bounds. Clamping to 0.");
            poiIndex = 0;
        }

        Debug.Log("FindPath: Building adjacency list...");
        adjacencyList = BuildAdjacencyList(obstacleMap);
        Debug.Log($"FindPath: Adjacency list built with {adjacencyList.Count} nodes");
        
        Debug.Log("FindPath: Starting coroutine to fetch EVA position...");
        //StartCoroutine(FetchEvaPositionAndProcessPath());
    }

    IEnumerator FetchEvaPositionAndProcessPath()
    {
        Debug.Log("========== FindPath: FETCHING EVA POSITION ==========");
        
        string evaPositionUrl = "http://127.0.0.1:8000/eva1/imu";
        Debug.Log($"FindPath: Fetching EVA position from: {evaPositionUrl}");

        using (UnityWebRequest request = UnityWebRequest.Get(evaPositionUrl))
        {
            Debug.Log("FindPath: Sending web request...");
            yield return request.SendWebRequest();
            Debug.Log($"FindPath: Request complete with status: {request.result}");

            if (request.result != UnityWebRequest.Result.Success) //temp switch up
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"EVA Position JSON received: {jsonResponse}");
                try
                {
                    EvaPositionResponse evaPosition = JsonUtility.FromJson<EvaPositionResponse>(jsonResponse);
                    if (evaPosition != null)
                    {
                        startNode = new Vector2Int(Mathf.RoundToInt(evaPosition.posx), Mathf.RoundToInt(evaPosition.posy));
                        Debug.Log($"FindPath: Start node set to: {startNode}");
                        ComputeAndDrawPath();
                    }
                    else
                    {
                        Debug.LogError("Failed to parse EVA position from JSON. Response was null.");
                    }
                }
                catch (System.Exception e) // Explicitly System.Exception
                {
                    Debug.LogError($"FindPath: Error parsing EVA position JSON: {e.Message}\nJSON: {jsonResponse}");
                }
            }
            else
            {
                Debug.LogError($"FindPath: Error fetching EVA position: {request.error}. Using default start (0,0) for testing.");
                startNode = new Vector2Int(5, 7);
                ComputeAndDrawPath();
            }

        }
    }

    void ComputeAndDrawPath()
    {
        Debug.Log("========== FindPath: COMPUTING AND DRAWING PATH ==========");
        
        if (adjacencyList == null || adjacencyList.Count == 0)
        {
            Debug.LogError("Adjacency list is not built. Cannot compute path.");
            return;
        }

        Vector2Int endNode = pois[poiIndex];
        Debug.Log($"Computing path from {startNode} to {endNode}");

        try
        {
            List<Vector2Int> path = Dijkstra(adjacencyList, startNode, endNode);
            if (path != null && path.Count > 0)
            {
                Debug.Log($"Path found with {path.Count} points.");
                
                // Log all coordinates in the path
                Debug.Log("========== FindPath: PATH COORDINATES ==========");
                string pathCoordinates = "";
                foreach (Vector2Int point in path)
                {
                    pathCoordinates += $"({point.x}, {point.y}) → ";
                }
                // Remove trailing arrow from last point
                if (pathCoordinates.Length > 3)
                    pathCoordinates = pathCoordinates.Substring(0, pathCoordinates.Length - 3);
                Debug.Log(pathCoordinates);
                
                pathDrawer.DrawPath(path);
            }
            else
            {
                Debug.LogWarning($"No path found from {startNode} to {endNode} or path was empty.");
            }
        }
        catch (System.Exception e) // Explicitly System.Exception
        {
            Debug.LogError($"Error during Dijkstra pathfinding: {e.Message}");
        }
    }

    private Dictionary<Vector2Int, List<Vector2Int>> BuildAdjacencyList(List<List<int>> grid)
    {
        var adjList = new Dictionary<Vector2Int, List<Vector2Int>>();
        if (grid == null || grid.Count == 0) return adjList;

        int height = grid.Count;
        int width = grid[0].Count;

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[y][x] == 1)
                    continue;

                var currentNode = new Vector2Int(x, y);
                var neighbors = new List<Vector2Int>();
                adjList[currentNode] = neighbors;

                foreach (var dir in directions)
                {
                    int nx = x + dir.x;
                    int ny = y + dir.y;

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && grid[ny][nx] == 0)
                    {
                        neighbors.Add(new Vector2Int(nx, ny));
                    }
                }
            }
        }
        
        // Log the adjacency list for debugging
        Debug.Log("========== FindPath: ADJACENCY LIST DETAILS ==========");
        foreach (var node in adjList.Keys)
        {
            string neighbors = "";
            foreach (var neighbor in adjList[node])
            {
                neighbors += neighbor.ToString() + ", ";
            }
            Debug.Log($"Node {node} connects to: {neighbors}");
        }
        
        return adjList;
    }

    private List<Vector2Int> Dijkstra(Dictionary<Vector2Int, List<Vector2Int>> graph, Vector2Int start, Vector2Int end)
    {
        if (!graph.ContainsKey(start) || !graph.ContainsKey(end))
        {
            Debug.LogError($"Start ({start}) or End ({end}) node not in graph. Start in graph: {graph.ContainsKey(start)}, End in graph: {graph.ContainsKey(end)}");
            int mapHeight = obstacleMap.Count;
            if (mapHeight == 0) return null; // Avoid error if obstacleMap is empty
            int mapWidth = obstacleMap[0].Count;
            if (mapWidth == 0) return null; // Avoid error if obstacleMap rows are empty

            if (start.x < 0 || start.x >= mapWidth || start.y < 0 || start.y >= mapHeight || obstacleMap[start.y][start.x] == 1)
                Debug.LogError($"Start node {start} is invalid or an obstacle.");
            if (end.x < 0 || end.x >= mapWidth || end.y < 0 || end.y >= mapHeight || obstacleMap[end.y][end.x] == 1)
                Debug.LogError($"End node {end} is invalid or an obstacle.");
            return null;
        }

        var distances = new Dictionary<Vector2Int, float>();
        var previousNodes = new Dictionary<Vector2Int, Vector2Int?>();
        var nodesToVisit = new List<Vector2Int>();

        foreach (var node in graph.Keys)
        {
            distances[node] = float.PositiveInfinity;
            previousNodes[node] = null;
            nodesToVisit.Add(node);
        }
        distances[start] = 0;

        while (nodesToVisit.Count > 0)
        {
            nodesToVisit.Sort((a, b) => distances[a].CompareTo(distances[b]));
            Vector2Int currentNode = nodesToVisit[0];
            nodesToVisit.RemoveAt(0);

            if (currentNode == end)
            {
                var path = new List<Vector2Int>();
                Vector2Int? current = end;
                while (current.HasValue)
                {
                    path.Add(current.Value);
                    current = previousNodes[current.Value];
                }
                path.Reverse();
                return path.Count > 0 && path[0] == start ? path : null;
            }

            if (distances[currentNode] == float.PositiveInfinity)
                break;

            if (!graph.ContainsKey(currentNode) || graph[currentNode] == null) continue;

            foreach (var neighbor in graph[currentNode])
            {
                float newDist = distances[currentNode] + Vector2Int.Distance(currentNode, neighbor);
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previousNodes[neighbor] = currentNode;
                }
            }
        }
        return null;
    }
}