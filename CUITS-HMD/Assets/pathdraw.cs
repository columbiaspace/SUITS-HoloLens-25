using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // For NavMesh pathfinding

public class PathDrawer : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // Player/camera transform 
    public Transform[] destinations;        // Size = 3, the POIs you want to path to
    public TSS_DATA tssData;                // Reference to TSS_DATA component to access position data
    public GameObject mapObject;            // Reference to the map game object

    [Header("Line Settings")]
    public Material pathMaterial;           // A simple unlit material
    public float lineWidth = 0.01f;         // How thick the line is (smaller for map surface)
    public float lineHeight = 0.001f;       // How far above map surface to place the lines
    
    [Header("Path Settings")]
    public float pathUpdateInterval = 0.5f; // How often to recalculate paths (seconds)
    public Color[] pathColors;              // Optional: different colors for each path

    // Unity scaling factor - adjust based on your world scale and the scale of your map
    [Header("Position Scaling")]
    public Vector2 positionScaleFactor = new Vector2(1.0f, 1.0f);
    public Vector2 positionOffset = new Vector2(0.0f, 0.0f);

    private List<LineRenderer> pathLines = new List<LineRenderer>();
    private float nextPathUpdate = 0f;
    private Renderer mapRenderer;

    void Start()
    {
        // Ensure we have the TSS_DATA component
        if (tssData == null)
        {
            tssData = FindObjectOfType<TSS_DATA>();
            if (tssData == null)
            {
                Debug.LogWarning("PathDrawer: TSS_DATA component not found in the scene!");
            }
        }
        
        // Find map object if not assigned
        if (mapObject == null)
        {
            mapObject = GameObject.Find("Map");
            if (mapObject == null)
            {
                Debug.LogError("PathDrawer: No Map object found! Please assign the map object in the inspector.");
                return;
            }
        }
        
        // Get the renderer for the map
        mapRenderer = mapObject.GetComponent<Renderer>();
        if (mapRenderer == null)
        {
            Debug.LogError("PathDrawer: Map object doesn't have a Renderer component!");
            return;
        }

        // Create one LineRenderer per destination
        for (int i = 0; i < destinations.Length; i++)
        {
            GameObject go = new GameObject($"PathLine_{i + 1}");
            go.transform.parent = mapObject.transform;  // Parent to map object

            var lr = go.AddComponent<LineRenderer>();
            lr.material = pathMaterial;
            lr.startWidth = lr.endWidth = lineWidth;
            lr.useWorldSpace = true;
            lr.positionCount = 0;                      // We'll set this dynamically based on path
            
            // Set color if available
            if (pathColors != null && i < pathColors.Length)
            {
                lr.startColor = lr.endColor = pathColors[i];
            }
            else
            {
                // Default colors if none provided
                Color[] defaultColors = { Color.red, Color.green, Color.blue };
                lr.startColor = lr.endColor = defaultColors[i % defaultColors.Length];
            }
            
            // Important: Set sorting parameters to ensure the paths appear above the map
            lr.sortingOrder = 1;
            lr.renderingLayerMask = 1;
            
            pathLines.Add(lr);
        }
        
        // Calculate paths immediately at start
        CalculatePaths();
    }

    void Update()
    {
        if (mapObject == null) return;
        
        // Update paths periodically
        if (Time.time > nextPathUpdate)
        {
            CalculatePaths();
            nextPathUpdate = Time.time + pathUpdateInterval;
        }
    }
    
    Vector3 ConvertTSSPositionToMapPosition(double tssPosX, double tssPosY)
    {
        // Find the map's bounds
        Bounds mapBounds = mapRenderer.bounds;
        
        // Convert from TSS position coordinates to relative position (0-1)
        float normalizedX = (float)tssPosX * positionScaleFactor.x + positionOffset.x;
        float normalizedZ = (float)tssPosY * positionScaleFactor.y + positionOffset.y;
        
        // Map to the physical map's bounds
        float x = mapBounds.min.x + normalizedX * mapBounds.size.x;
        float z = mapBounds.min.z + normalizedZ * mapBounds.size.z;
        
        // Use the map's Y position plus a small offset to float above the surface
        float y = mapObject.transform.position.y + lineHeight;
        
        return new Vector3(x, y, z);
    }
    
    void CalculatePaths()
    {
        if (mapObject == null) return;
        
        // Get positions from TSS backend if available
        if (tssData != null && tssData.imu != null && tssData.imu.dcu != null)
        {
            // Get the user's position from the IMU data
            Vector3 playerPosition;

            // If we're using EVA1 position
            if (tssData.imu.dcu.eva1 != null)
            {
                playerPosition = ConvertTSSPositionToMapPosition(
                    tssData.imu.dcu.eva1.posx,
                    tssData.imu.dcu.eva1.posy
                );
            }
            else
            {
                // Fallback to using the player transform's position
                // Project the player's position onto the map
                Vector3 projectedPosition = player.position;
                projectedPosition.y = mapObject.transform.position.y + lineHeight;
                playerPosition = projectedPosition;
            }

            DrawPathsToDestinations(playerPosition);
        }
        else
        {
            // Fallback to using the player transform's position
            Vector3 projectedPosition = player.position;
            projectedPosition.y = mapObject.transform.position.y + lineHeight;
            DrawPathsToDestinations(projectedPosition);
        }
    }
    
    void DrawPathsToDestinations(Vector3 startPosition)
    {
        // For each destination, calculate path
        for (int i = 0; i < destinations.Length; i++)
        {
            if (destinations[i] == null || pathLines.Count <= i) continue;
            
            // Project destination point onto the map
            Vector3 destPosition = destinations[i].position;
            destPosition.y = mapObject.transform.position.y + lineHeight;
            
            // Create a new path
            NavMeshPath path = new NavMeshPath();
            
            // Calculate the path
            bool pathFound = NavMesh.CalculatePath(
                startPosition,
                destPosition,
                NavMesh.AllAreas,
                path
            );
            
            // Update LineRenderer based on path status
            if (pathFound && path.status != NavMeshPathStatus.PathInvalid)
            {
                // Process the path corners to be just above the map
                Vector3[] corners = path.corners;
                for (int j = 0; j < corners.Length; j++)
                {
                    corners[j].y = mapObject.transform.position.y + lineHeight;
                }
                
                // Set the number of points in the line
                pathLines[i].positionCount = corners.Length;
                
                // Set the positions
                pathLines[i].SetPositions(corners);
            }
            else
            {
                // Draw a straight line if no path found
                pathLines[i].positionCount = 2;
                pathLines[i].SetPosition(0, startPosition);
                pathLines[i].SetPosition(1, destPosition);
            }
        }
    }
    
    // You can call this from other scripts when you need to force a path update
    public void RefreshPaths()
    {
        CalculatePaths();
        nextPathUpdate = Time.time + pathUpdateInterval;
    }
}
