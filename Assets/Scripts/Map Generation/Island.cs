using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public bool isTriggered = false;

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

    public List<Collider> areaColliders = new List<Collider>();

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

    public void spawnEnemies(int count)
    {
        if (isTriggered) return;
        int c = 0;
        while (c < count)
        {
            Vector2 p = point + (Random.insideUnitCircle * farEdgeMax);
            Vector3 v = new Vector3(p.x, 10, p.y);
            Debug.DrawRay(v + transform.position, Vector3.down, Color.red, 1000f);
            if (Physics.Raycast(v + transform.position, Vector3.down, out RaycastHit hit, 20))
            {
                if (hit.point.y < 0.1f && !Physics.CheckSphere(hit.point, 25, LayerMask.NameToLayer("GroundObjects")))
                {
                    PrefabManager.instance.spawnEnemy(hit.point);
                    c++;
                }
            }
        }
        isTriggered = true;

    }

    public void spawnBoss()
    {
        Vector3 v = new Vector3(point.x, 10, point.y);
        Debug.DrawRay(v + transform.position, Vector3.down, Color.red, 1000f);
        if (Physics.Raycast(v + transform.position, Vector3.down, out RaycastHit hit, 20))
        {
            if (hit.point.y < 0.1f && !Physics.CheckSphere(hit.point, 25, LayerMask.NameToLayer("GroundObjects")))
            {
                PrefabManager.instance.spawnBoss(hit.point);
            }
        }
    }

    public void spawnTrees()
    {
        for (int i = 0; i < (int)Random.Range(radius/4, radius); i++)
        {
            int count = 0;
            Vector2 p = point + (Random.insideUnitCircle * farEdgeMax);
            Vector3 v = new Vector3(p.x, 10, p.y);
            Debug.DrawRay(v + transform.position, Vector3.down, Color.red, 1000f);
            if (Physics.Raycast(v + transform.position, Vector3.down, out RaycastHit hit, 20))
            {
                if (!Physics.CheckSphere(hit.point, 25, LayerMask.NameToLayer("GroundObjects")) && hit.point.y < 0.1f)
                {
                    PrefabManager.instance.spawnRandomTree(hit.point, transform);
                    count++;
                }
            }
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;
        if (collision.gameObject.tag != "Player") return;


        CameraController.instance.idealSize = radius;
    }
}
