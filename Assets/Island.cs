using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Island : MonoBehaviour
{
    public int vertCount;
    public Vector2 point;
    public Vector3 center;
    public float radius;
    public float minRandom;
    public float maxRandom;
    public float maxDistance;

    public bool isStart = false;
    public bool isEnd = false;

    public float[] layerThicknesses;
    public float farEdgeMax = 0;

    //pathfinding
    public float gCost;
    public float hCost;
    public bool isWalkable = true;
    public float fCost { get { return gCost + hCost; } }

    public List<Island> connections = new List<Island>();
    public Island prev;


    public List<Island> next = new List<Island>();
    public List<Island> previous = new List<Island>();

    public void createIslandMesh()
    {
        Mesh mesh = IslandMeshGenerator.getMesh(this);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        maxDistance = radius + farEdgeMax;
    }

    public void createIslandParams(Vector2 pos, int vc, float r, float randomness, float[] layers)
    {
        vertCount = vc;
        center = new Vector3(pos.x, 0, pos.y);
        point = pos;
        radius = r;
        minRandom = radius - randomness*radius;
        maxRandom = radius + randomness*radius;
        layerThicknesses = layers;
        maxDistance = r + maxRandom;
        farEdgeMax = minRandom;
        isWalkable = true;
    }



    public float getTopNoise()
    {
        float n = Random.Range(minRandom, maxRandom);
        if (n > farEdgeMax) { farEdgeMax = n; }
        return n;
    }

    public float getMiddleNoise()
    {
        return Random.Range(minRandom/2, maxRandom/2);
    }

    public void delete()
    {
        Destroy(gameObject);
    }
}
