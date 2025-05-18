using UnityEngine;

public class MapLine : MonoBehaviour
{
    [Header("Line Points")]
    public Transform pointA;               // First point on the map
    public Transform pointB;               // Second point on the map
    
    [Header("Line Appearance")]
    public Color lineColor;  // Color of the line
    public float lineWidth = 0.01f;        // Width of the line
    public float lineHeight = 0.001f;      // How high above the map the line should appear
    
    private LineRenderer lineRenderer;
    
    void Start()
    {
        // Create a LineRenderer if it doesn't exist
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();

            print("linerenderer is null");
        }
        Debug.Log("MapLine: One or both points are not assigned!");
        // Set up the basic properties of the line
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.positionCount = 2;

        // Create a simple material for the line
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = lineColor;

        // Make sure the line renders on top of other objects
        lineRenderer.sortingOrder = 1;

        // Draw the initial line
        UpdateLine();
    }
    
    void Update()
    {
        // Update the line if either point has moved
        UpdateLine();
    }
    
    void UpdateLine()
    {
        // Make sure both points are assigned
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("MapLine: One or both points are not assigned!");
            return;
        }
        
        // Set the line positions
        Vector3 posA = pointA.position;
        Vector3 posB = pointB.position;
        
        // Adjust height to float slightly above the map
        posA.y += lineHeight;
        posB.y += lineHeight;
        
        lineRenderer.SetPosition(0, posA);
        lineRenderer.SetPosition(1, posB);
    }
    
    // Public method to set points programmatically
    public void SetPoints(Vector3 start, Vector3 end)
    {
        // Create GameObjects for the points if they don't exist
        if (pointA == null)
        {
            GameObject objA = new GameObject("PointA");
            pointA = objA.transform;
            objA.transform.parent = this.transform;
        }
        
        if (pointB == null)
        {
            GameObject objB = new GameObject("PointB");
            pointB = objB.transform;
            objB.transform.parent = this.transform;
        }
        
        // Set positions
        pointA.position = start;
        pointB.position = end;
        
        // Update the line
        UpdateLine();
    }
}