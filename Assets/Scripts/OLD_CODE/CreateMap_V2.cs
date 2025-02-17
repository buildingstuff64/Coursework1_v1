using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using Unity.VisualScripting;

public class CreateMap_V2 : MonoBehaviour
{
    class IslandNode
    {
        public Vector3 pos;
        public List<IslandNode> connections;
        public float radius;
    }

    List<IslandNode> nodes = new List<IslandNode>();

    private void Start()
    {
        // Define a set of points
        Vector2[] points = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0.5f, 0.5f)
        };

        // Convert Unity's Vector2 to DelaunatorSharp's IPoint[]
        var delaunayPoints = points.ToPoints();

        // Create the Delaunay triangulation
        Delaunator delaunator = new Delaunator(delaunayPoints);

        // Iterate through the triangles
        foreach (var triangle in delaunator.GetTriangles())
        {
            Debug.Log($"Triangle: {triangle}");
        }

        // Optional: Draw triangles in Unity for visualization
        foreach (var edge in delaunator.GetEdges())
        {
            Debug.DrawLine(edge.Q.ToVector3(), edge.P.ToVector3());
        }
    }

    private void generateNodes(IslandNode start, IslandNode end)
    {
        // Define a set of points
        Vector2[] points = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0.5f, 0.5f)
        };

        // Convert Unity's Vector2 to DelaunatorSharp's IPoint[]
        var delaunayPoints = points.ToPoints();

        // Create the Delaunay triangulation
        Delaunator delaunator = new Delaunator(delaunayPoints);

        // Iterate through the triangles
        foreach (var triangle in delaunator.GetTriangles())
        {
            Debug.Log($"Triangle: {triangle}");
        }

        // Optional: Draw triangles in Unity for visualization
        foreach (var edge in delaunator.GetEdges())
        {
            print("TEST");
            // Log the values of the edge
            if (start != null && end != null)
            {
                Debug.Log($"Edge from ({edge.P.X}, {edge.P.Y}) to ({edge.Q.X}, {edge.Q.Y})");
            }
            else
            {
                Debug.LogError("Edge has null points!");
            }
            print("Test");
        }

    }

}
