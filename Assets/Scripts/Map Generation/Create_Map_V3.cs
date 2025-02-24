using DelaunatorSharp;
using DelaunatorSharp.Unity.Extensions;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class Create_Map_V3 : MonoBehaviour
{
    public static Create_Map_V3 instance;

    public GameObject islandFab;
    public GameObject cube;

    [Header("MapGeneration")]
    public float islandAreaRadius;
    public int islandIterationCount;
    [Space(10)]


    [Header("IslandGeneration")]
    public float islandRadiusMin;
    public float islandRadiusMax;
    public int islandVertexCount;
    public float randomness;
    public int pathBlockingIterations;

    [Header("Start Island")]
    public float startIslandSize = 5f;

    [Header("End Island")]
    public float endIslandSize = 75f;

    private NavMeshSurface navmesh;
    public List<Island> finalIslands = new List<Island>();

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        //create islands
        Island start = spawnIsland(Vector2.zero, startIslandSize);
        Island end = spawnIsland(Vector2.up * islandAreaRadius * 2, endIslandSize);
        List<Island> islands = spawnIslands(start, end, islandIterationCount);
        List<Vector2> points = new List<Vector2>();
        foreach (Island island in islands)
        {
            points.Add(island.point);
        }

        //trianglate all the island s with delauny
        Delaunator delaunator = new Delaunator(points.ToPoints());
        List<DelaunatorSharp.Edge> edges = new List<DelaunatorSharp.Edge>();
        foreach (var edge in delaunator.GetEdges())
        {
            Vector3 p = edge.P.ToVector3() * 2;
            p.z = p.y;
            p.y = 0;
            Island ip = getIslandFromRay(p);
            
            Vector3 q = edge.Q.ToVector3() * 2;
            q.z = q.y;
            q.y = 0;
            Island iq = getIslandFromRay(q);

            //Debug.DrawLine(ip.center, iq.center, Color.magenta, 1000f);    

            ip.connections.Add(iq);
            iq.connections.Add(ip);

        }

        //find mulitple paths though-out the islands
        Pathfinding pathfinding = new Pathfinding(start, end);

        List< List<Island>> paths = createRandomPaths(pathfinding, pathBlockingIterations);
        foreach (List<Island> path in paths)
        {
            foreach (Island p in path)
            {
                p.connections.Clear();
                p.isWalkable = true;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i].center, path[i + 1].center, Color.red, 1000f);

                path[i].next.Add(path[i + 1]);
                path[i + 1].previous.Add(path[i]);

            }
        }

        //cull all not connected islands and merge into a singluar list
        finalIslands = new List<Island>();
        foreach (List<Island> path in paths) { finalIslands.AddRange(path); }
        finalIslands = finalIslands.Distinct().ToList();

        foreach (Island i in islands)
        {
            if (!finalIslands.Contains(i))
            {
                DestroyImmediate(i.gameObject);
            }
        }

        foreach (Island i in finalIslands)
        {

            foreach (Island next in i.next)
            {
                createBridge(i, next);
            }
            if (!i.isStart && !i.isEnd)
            {
                i.spawnTrees();
            }
        }

        //create and build the navmesh
        navmesh = GetComponent<NavMeshSurface>();
        navmesh.BuildNavMesh();

        // spawnEnemies();

    }

    void spawnEnemies()
    {
        print(finalIslands.Count);
        foreach (Island island in finalIslands)
        {
            if (!island.isEnd && !island.isStart)
            {
                int count = Mathf.RoundToInt(island.farEdgeMax / 10);
                for (int i = 0; i < count; i++)
                {
                    Vector2 p = island.point + (Random.insideUnitCircle * island.farEdgeMax);
                    Vector3 v = new Vector3(p.x, 10, p.y);
                    Debug.DrawRay(v + island.transform.position, Vector3.down, Color.red, 1000f);
                    if (Physics.Raycast(v + island.transform.position, Vector3.down, out RaycastHit hit, 20))
                    {
                        PrefabManager.instance.spawnEnemy(hit.point);
                    }
                    print("test");
                }
            }
        }
    }

    void createBridge(Island from, Island to)
    {
        Vector3 midPoint = Vector3.Lerp(from.center, to.center, 0.5f);
        GameObject g = Instantiate(cube, midPoint, Quaternion.identity, transform);
        g.layer = 7;
        g.transform.LookAt(to.center);
        g.transform.localScale = new Vector3(10, 1, Vector3.Distance(from.center, to.center));
        g.transform.position += Vector3.down * 0.6f;
        g.AddComponent<Bridge>();
        Bridge bridge = g.GetComponent<Bridge>();
        bridge.from = from;
        bridge.to = to;
    }

    List<List<Island>> createRandomPaths(Pathfinding pathfinding, int iterations)
    {
        List<Island> final = new List<Island>();
        List<List<Island>> paths = new List<List<Island>>();
        final.AddRange(pathfinding.findPath());
        for (int i = 0; i < iterations; i++)
        {
            Island remove = final[Random.Range(0, final.Count)];
            remove.isWalkable = false;

            List<Island> path = pathfinding.findPath();
            if (path == null || path.Count<1)
            {
                remove.isWalkable = true;
            }
            else
            {
                final.AddRange(path);
                paths.Add(path);
            }

            final = final.Distinct().ToList();

        }
        return paths;
    }

    Island getIslandFromRay(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up + Vector3.back*0.001f, Vector3.down, out RaycastHit hit))
        {
            //Debug.DrawLine(pos + (Vector3.up * 100), pos, Color.blue, 1000f);
            GameObject go = hit.collider.gameObject;
            go.GetComponent<Island>().center = pos;
            Island i = go.GetComponent<Island>();
            if (i == null) Debug.Log(pos);
            return i;
        }
        return null;
    }


    Island spawnIsland(Vector2 pos)
    {
        GameObject go = Instantiate(islandFab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, transform);
        go.layer = 7;
        Island island = go.GetComponent<Island>();
        float radius = Random.Range(islandRadiusMin, islandRadiusMax);
        island.createIslandParams(pos, islandVertexCount, radius, randomness, new float[] { 2f, radius, 2.4f });
        island.createIslandMesh();
        go.name = island.point.ToString();
        return island;
    }

    Island spawnIsland(Vector2 pos, float size)
    {
        GameObject go = Instantiate(islandFab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, transform);
        go.layer = 7;
        Island island = go.GetComponent<Island>();
        island.createIslandParams(pos, islandVertexCount, size, randomness, new float[] { 2f, size, 2.4f });
        island.createIslandMesh();
        go.name = island.point.ToString();
        return island;
    }

    List<Island> spawnIslands(Island start, Island end, int count)
    {
        List<Island> islands = new List<Island>{ start, end };
        Vector2 midPoint = Vector2.Lerp(start.point, end.point, 0.5f);
        islands[0].isStart = true;
        islands[1].isEnd = true;
        for (int i = 0; i < count; i++)
        {
            Island island = spawnIsland((Random.insideUnitCircle * islandAreaRadius) + midPoint);
            //Debug.DrawLine(island.transform.position * 2, island.transform.position * 2 + Vector3.up * 2, Color.green, 1000f);
            bool x = false;
            for (int j = 0; j < islands.Count; j++)
            {
                if (Vector3.Distance(islands[j].transform.position, island.transform.position) < island.farEdgeMax + islands[j].farEdgeMax)
                {
                    DestroyImmediate(island.gameObject);
                    x = true;
                    break;
                }
            }
            if (x) continue;  
            islands.Add(island);
        }
        return islands;
    }

}
