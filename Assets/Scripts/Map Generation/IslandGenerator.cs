using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/*
public class IslandGenerator : MonoBehaviour
{
    Mesh mesh;

    List<Vector3> Vertices = new List<Vector3>();
    List<int> TrianglesMatA = new List<int>();
    List<int> TrianglesMatB = new List<int>();
    int currentTriIndex = 0;
    Island island;

    public void generateIsland(float radius, float randomness, int vertCount, string name)
    {
        mesh = new Mesh();
        mesh.name = name;
        mesh.subMeshCount = 2;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        createIslandMesh();

        mesh.SetVertices(Vertices);
        mesh.SetTriangles(TrianglesMatA, 0);
        mesh.SetTriangles(TrianglesMatB, 1);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void createIslandMesh()
    {
        createVertices(center, vertCount, radius, randomness);

        for (int i = 0; i < vertCount; i++)
        {
            TrianglesMatA.Add(currentTriIndex);
            TrianglesMatA.Add(currentTriIndex + i + 1);
            TrianglesMatA.Add(currentTriIndex + i);

        }

        TrianglesMatA.Add(currentTriIndex);
        TrianglesMatA.Add(currentTriIndex + 1);
        TrianglesMatA.Add(currentTriIndex + vertCount);


        //create sides

        //sides triangles
        createSide(vertCount);
        currentTriIndex += vertCount;
        createSide(vertCount);
        currentTriIndex += vertCount;

        for (int i = 0;i < vertCount - 1;i++)
        {
            TrianglesMatB.Add(currentTriIndex + i + 1);
            TrianglesMatB.Add(currentTriIndex + i + 2);
            TrianglesMatB.Add(Vertices.Count-1);
        }

        TrianglesMatB.Add(currentTriIndex + vertCount);
        TrianglesMatB.Add(currentTriIndex + 1);
        TrianglesMatB.Add(Vertices.Count - 1);

    }

    void createSide(int vertCount)
    {
        for (int i = 0; i < vertCount - 1; i++)
        {
            TrianglesMatB.Add(currentTriIndex + i + 1);
            TrianglesMatB.Add(currentTriIndex + i + 2);
            TrianglesMatB.Add(currentTriIndex + i + vertCount + 1);

            TrianglesMatB.Add(currentTriIndex + i + vertCount + 2);
            TrianglesMatB.Add(currentTriIndex + i + vertCount + 1);
            TrianglesMatB.Add(currentTriIndex + i + 2);

        }

        TrianglesMatB.Add(currentTriIndex + 1);
        TrianglesMatB.Add(currentTriIndex + vertCount * 2);
        TrianglesMatB.Add(currentTriIndex + vertCount);

        TrianglesMatB.Add(currentTriIndex + 1);
        TrianglesMatB.Add(currentTriIndex + vertCount + 1);
        TrianglesMatB.Add(currentTriIndex + vertCount * 2);
    }


    void createVertices()
    {
        Vertices.Add(island.tr);

        List<Vector3> topLayer = createLayerPositions(center, vertCount, radius, min, max);
        List<Vector3> sideLayer = new List<Vector3>();
        for (int i = 0; i < topLayer.Count; i++) sideLayer.Add(topLayer[i] + Vector3.down * 1f);

        Vertices.AddRange(topLayer);
        Vertices.AddRange(sideLayer);

        List<Vector3> middleLayer = createLayerPositions(center + Vector3.down*(radius), vertCount, radius / 2, min, max);
        Vertices.AddRange(middleLayer);

        Vertices.Add(center + new Vector3(Random.Range(0, 1), -radius*2.4f, Random.Range(0, 1)));



    }

    List<Vector3> createLayerPositions(Vector3 center, int vertCount, float radius)
    {
        List<Vector3> verts = new List<Vector3>();
        for (int i = 0; i < vertCount; i++)
        {
            float c = (360 / vertCount);
            float deg = c * i - c / 2;
            float rad = Mathf.PI / 180 * deg;

            verts.Add(center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * (radius));
        }
        return verts;
    }

    float getNoise(float min, float max)
    {
        return Random.Range(min, max);
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 i in Vertices)
        {
            Gizmos.DrawSphere(i, 0.5f);
        }
    }

}
*/

static class IslandMeshGenerator
{
    public static Mesh getMesh(Island island)
    {
        Mesh mesh = new Mesh();
        mesh.name = island.transform.position.ToString();
        mesh.subMeshCount = 2;

        List<Vector3> vertices = generateVertices(island);
        List<List<int>> triangles = generateTriangles(island, vertices.Count);

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles[0], 0);
        mesh.SetTriangles(triangles[1], 1);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;

    }


    private static List<Vector3> generateVertices(Island island)
    {
        List<Vector3> v = new List<Vector3>() { island.transform.position };

        List<Vector3> topLayer = generateIslandLayer(island, island.transform.position, island.radius, island.getTopNoise);
        List<Vector3> sideLayer = new List<Vector3>();
        for (int i = 0; i < topLayer.Count; i++) sideLayer.Add(topLayer[i] + Vector3.down * island.layerThicknesses[0]);

        v.AddRange(topLayer);
        v.AddRange(sideLayer);

        List<Vector3> middleLayer = generateIslandLayer(island, (island.transform.position + Vector3.down * (island.layerThicknesses[1])), island.radius / 2, island.getMiddleNoise);
        v.AddRange(middleLayer);

        v.Add(island.transform.position + new Vector3(Random.Range(0, 1), -island.radius * island.layerThicknesses[2], Random.Range(0, 1)));;
        return v;

    }

    private static List<Vector3> generateIslandLayer(Island island, Vector3 center, float radius, System.Func<float> noise)
    {
        List<Vector3> verts = new List<Vector3>();
        for (int i = 0; i < island.vertCount; i++)
        {
            float c = (360 / island.vertCount);
            float deg = c * i - c / 2;
            float rad = Mathf.PI / 180 * deg;

            verts.Add(center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * (radius + noise.Invoke()));
        }
        return verts;
    }

    private static List<List<int>>generateTriangles(Island island, int verticeCount)
    {
        List<int> TriA = new List<int>();
        List<int> TriB = new List<int>();
        int index = 0;

        for (int i = 0; i < island.vertCount; i++)
        {
            TriA.Add(index);
            TriA.Add(index + i + 1);
            TriA.Add(index + i);

        }

        TriA.Add(index);
        TriA.Add(index + 1);
        TriA.Add(index + island.vertCount);


        //create sides

        //sides triangles

        for (int i = 0; i < island.vertCount - 1; i++)
        {
            TriB.Add(index + i + 1);
            TriB.Add(index + i + 2);
            TriB.Add(index + i + island.vertCount + 1);

            TriB.Add(index + i + island.vertCount + 2);
            TriB.Add(index + i + island.vertCount + 1);
            TriB.Add(index + i + 2);

        }

        TriB.Add(index + 1);
        TriB.Add(index + island.vertCount * 2);
        TriB.Add(index + island.vertCount);

        TriB.Add(index + 1);
        TriB.Add(index + island.vertCount + 1);
        TriB.Add(index + island.vertCount * 2);



        index += island.vertCount;

        for (int i = 0; i < island.vertCount - 1; i++)
        {
            TriB.Add(index + i + 1);
            TriB.Add(index + i + 2);
            TriB.Add(index + i + island.vertCount + 1);

            TriB.Add(index + i + island.vertCount + 2);
            TriB.Add(index + i + island.vertCount + 1);
            TriB.Add(index + i + 2);

        }

        TriB.Add(index + 1);
        TriB.Add(index + island.vertCount * 2);
        TriB.Add(index + island.vertCount);

        TriB.Add(index + 1);
        TriB.Add(index + island.vertCount + 1);
        TriB.Add(index + island.vertCount * 2);

        index += island.vertCount;

        for (int i = 0; i < island.vertCount - 1; i++)
        {
            TriB.Add(index + i + 1);
            TriB.Add(index + i + 2);
            TriB.Add(verticeCount - 1);
        }

        TriB.Add(index + island.vertCount);
        TriB.Add(index + 1);
        TriB.Add(verticeCount - 1);

        return new List<List<int>> { TriA, TriB };
    }

}