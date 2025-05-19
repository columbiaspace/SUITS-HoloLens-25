using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float tileSize = 1f; // adjust based on your grid

    public void DrawPath(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0) return;

        lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            int y = path[i][0]; // row
            int x = path[i][1]; // column

            Vector3 worldPos = new Vector3(x * tileSize, 0.1f, y * tileSize); // lifted slightly for visibility
            lineRenderer.SetPosition(i, worldPos);
        }
    }
}
