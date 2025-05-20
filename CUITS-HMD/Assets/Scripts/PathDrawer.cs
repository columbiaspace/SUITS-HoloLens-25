using System.Collections.Generic;
using UnityEngine;


public class PathDrawer : MonoBehaviour
{
   public LineRenderer lineRenderer;
   public float tileSize = 1f; // adjust based on your grid
   public Vector3 offset = new Vector3(-11.5, 11.5, -1); // position offset for the entire path


   public bool useLocalSpace = true; // When true, positions are relative to this GameObject
  
   [Header("Visibility Settings")]
   public float lineWidth = 0.1f;  // Width of the line
   public float zOffset = 0.1f;    // How far in front of the slate to show the line
   public Color lineColor = Color.red; // Color of the line




   public void DrawPath(List<Vector2Int> path)
   {
       if (path == null || path.Count == 0) return;


       lineRenderer.positionCount = path.Count;


       for (int i = 0; i < path.Count; i++)
       {
           int y = path[i].y; // row
           int x = path[i].x; // column


           // Negate the y value to make increasing y go downward
           Vector3 position = new Vector3(x * tileSize, -y * tileSize, .1f); // lifted slightly for visibility
          
           // Apply the offset to position the path where it should be
           position += offset;
          
           // If using local space, LineRenderer will interpret positions as local to this GameObject
           if (useLocalSpace)
           {
               // Set positions directly in local space
               lineRenderer.useWorldSpace = false;
               lineRenderer.SetPosition(i, position);
           }
           else
           {
               // Convert to world space if needed
               lineRenderer.useWorldSpace = true;
               lineRenderer.SetPosition(i, transform.TransformPoint(position));
           }
          
           Debug.Log($"Path point {i}: Position = ({position.x}, {position.y}, {position.z})");


       }
   }
}






